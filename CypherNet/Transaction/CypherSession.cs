using CypherNet.Http;

namespace CypherNet.Transaction
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Dynamic;
    using Graph;
    using Queries;
    using Serialization;
    using StaticReflection;

    #endregion

    public class CypherSession : ICypherSession
    {

        private static readonly int[] MinimumVersionNumber = new[] {2, 0, 0};
       
        private static readonly string NodeVariableName = ReflectOn<CreateNodeResult>.Member(a => a.NewNode).Name;

        private static readonly string CreateNodeClauseFormat =
            String.Format(@"CREATE ({0}{{0}} {{1}}) RETURN {0} as {{2}}, id({0}) as {{3}}, labels({0}) as {{4}};",
                          NodeVariableName);

        private readonly string _uri;
        private readonly ICypherClientFactory _clientFactory;
        private readonly IWebSerializer _webSerializer;
        private readonly IEntityCache _entityCache;
        private readonly IWebClient _webClient;

        internal CypherSession(string uri)
        {
            _uri = uri;
            // I would prefer to inject these dependencies, however
            // I'm not sure how else we can ensure that an instance of CypherSession
            // shares the same instance of IEnityCache with its
            // IWebSerializer???
            _entityCache = new DictionaryEntityCache();
            _webSerializer = new DefaultJsonSerializer(_entityCache);
            _webClient = new WebClient(_webSerializer);
            _clientFactory = new CypherClientFactory(uri, _webClient);
        }

        internal void Connect()
        {
            ServiceRootResponse response = null;
            try
            {
                response = _webClient.GetAsync<ServiceRootResponse>(_uri).Result;
            }
            catch (Exception)
            {
                throw new NeoServerUnavalaibleExpcetion(_uri);
            }

            AssertVersion(response);
        }

        private void AssertVersion(ServiceRootResponse response)
        {
            var serverversion = response.Version;
            if (String.IsNullOrEmpty(serverversion))
            {
                throw new Exception("Cannot read Neo4j Server Version");
            }
            var versionNumberStrings = serverversion.Split(new[]{'.','-'}).Take(3).ToArray();
            for (var i = 0; i < versionNumberStrings.Count(); i++)
            {
                var versionNumberString = versionNumberStrings[i];
                var versionNumber = 0;
                if (!int.TryParse(versionNumberString, out versionNumber))
                {
                    throw new Exception("Invalid Neo4j Server Version: " + serverversion);
                }
                if (versionNumber < MinimumVersionNumber[i])
                {
                    throw new Exception(String.Format("Incompatible Neo4j Server Version: {0}. Cypher.Net is currently only compatible with Neo4j versions {1} and above", serverversion, String.Join(".", MinimumVersionNumber)));
                }
                else if (versionNumber > MinimumVersionNumber[i])
                {
                    return;
                }
            }

        }


        #region ICypherSession Members

        public ICypherQueryStart<TVariables> BeginQuery<TVariables>()
        {
            return new FluentCypherQueryBuilder<TVariables>(_clientFactory);
        }

        public ICypherQueryStart<TVariables> BeginQuery<TVariables>(
            Expression<Func<ICypherPrototype, TVariables>> variablePrototype)
        {
            return new FluentCypherQueryBuilder<TVariables>(_clientFactory);
        }

        public Node CreateNode(object properties)
        {
            return CreateNode(properties, null);
        }

        public Node CreateNode(object properties, string label)
        {
            var props = _webSerializer.Serialize(properties);
            var propNames = new EntityReturnColumns(NodeVariableName);
            var clause = String.Format(CreateNodeClauseFormat, String.IsNullOrEmpty(label) ? "" : ":" + label, props,
                                       propNames.PropertiesPropertyName, propNames.IdPropertyName,
                                       propNames.LabelsPropertyName);
            var endpoint = _clientFactory.Create();
            var result = endpoint.ExecuteQuery<CreateNodeResult>(clause);
            var node = result.First().NewNode;
            return node;
        }

        public Node GetNode(long id)
        {
            if (_entityCache.Contains(id))
            {
                return _entityCache.GetEntity(id) as Node;
            }
            else
            {
                var query = BeginQuery(n => new {newNode = n.Node})
                    .Start(v => v.StartAtId(v.Vars.newNode, id))
                    .Return(v => new {v.newNode})
                    .Fetch();
                var firstRow = query.FirstOrDefault();
                return firstRow == null ? null : firstRow.newNode;
            }
        }

        public void Delete(long nodeId)
        {
            BeginQuery(n => new {newNode = n.Node})
                .Start(v => v.StartAtId(v.Vars.newNode, nodeId))
                .Delete(v => v.newNode)
                .Execute();
            _entityCache.Remove(nodeId);
        }

        public void Delete(Node node)
        {
            Delete(node.Id);
        }

        private static readonly MethodInfo SetMethodInfo = typeof (GraphEntityExtensions).GetMethod("Set", BindingFlags.Public | BindingFlags.Static);

        private static readonly PropertyInfo NewNodeProperty =
            (PropertyInfo) ReflectOn<CreateNodeResult>.Member(c => c.NewNode).MemberInfo;

        public void Save(Node node)
        {
            var props = node as IDynamicMetaData;
            var vals = props.GetAllValues();
            var setActions = new List<Expression<Action<CreateNodeResult>>>();
            foreach (var val in vals)
            {
                var param = Expression.Parameter(typeof (CreateNodeResult));
                var propType = val.Value.GetType();
                var method = SetMethodInfo.MakeGenericMethod(new[] {propType});
                var member = Expression.Property(param, NewNodeProperty);
                var call = Expression.Call(method, member, Expression.Constant(val.Key), Expression.Constant(val.Value));
                var lambda = Expression.Lambda<Action<CreateNodeResult>>(call, param);
                setActions.Add(lambda);
            }

            BeginQuery<CreateNodeResult>().Start(v => v.StartAtId(v.Vars.NewNode, node.Id))
                                          .Update(setActions.ToArray())
                                          .Execute();

        }

        #endregion

        internal class CreateNodeResult
        {
            public CreateNodeResult(Node newNode)
            {
                NewNode = newNode;
            }

            public Node NewNode { get; private set; }
        }

        public void Clear()
        {
            _entityCache.Clear();
        }
    }
}