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
        private static readonly string NodeVariableName = ReflectOn<CreateNodeResult>.Member(a => a.NewNode).Name;

        private static readonly string CreateNodeClauseFormat =
            String.Format(@"CREATE ({0}{{0}} {{1}}) RETURN {0} as {{2}}, id({0}) as {{3}}, labels({0}) as {{4}};",
                          NodeVariableName);

        private readonly ICypherClientFactory _clientFactory;
        private readonly IWebSerializer _webSerializer;
        private readonly IEntityCache _entityCache;

        internal CypherSession(string uri)
        {
            _entityCache = new DictionaryEntityCache();
            _webSerializer = new DefaultJsonSerializer(_entityCache);
            _clientFactory = new CypherClientFactory(uri, new WebClient(_webSerializer));
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
                    .Start(v => Start.At(v.newNode, id))
                    .Return(v => new {v.newNode})
                    .Fetch();
                var firstRow = query.FirstOrDefault();
                return firstRow == null ? null : firstRow.newNode;
            }
        }

        public void Delete(long nodeId)
        {
            BeginQuery(n => new {newNode = n.Node})
                .Start(v => Start.At(v.newNode, nodeId))
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

            BeginQuery<CreateNodeResult>().Start(v => Start.At(v.NewNode, node.Id))
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