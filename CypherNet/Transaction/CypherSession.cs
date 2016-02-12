using CypherNet.Configuration;

namespace CypherNet.Transaction
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    using CypherNet.Dynamic;
    using CypherNet.Graph;
    using CypherNet.Http;
    using CypherNet.Queries;
    using CypherNet.Serialization;

    using StaticReflection;

    #endregion

    public class CypherSession : ICypherSession
    {
        private const string CreateConstraintClauseFormat = "CREATE CONSTRAINT ON ({0}:{1}) ASSERT {0}.{2} IS UNIQUE";
        private const string DropConstraintClauseFormat = "DROP CONSTRAINT ON ({0}:{1}) ASSERT {0}.{2} IS UNIQUE";

        private const string CreateIndexClauseFormat = "CREATE INDEX ON :{0}({1})";
        private const string DropIndexClauseFormat = "DROP INDEX ON :{0}({1})";

        private static readonly int[] MinimumVersionNumber = new[] {2, 0, 0};
       
        private static readonly string NodeVariableName = ReflectOn<SingleNodeResult>.Member(a => a.NewNode).Name;

        private static readonly string CreateNodeClauseFormat =
            String.Format(@"CREATE ({0}{{0}} {{1}}) RETURN {0} as {{2}}, id({0}) as {{3}}, labels({0}) as {{4}};",
                          NodeVariableName);

        private readonly string _uri;
        private readonly IWebSerializer _webSerializer;
        private readonly IEntityCache _entityCache;
        private readonly IWebClient _webClient;

        internal CypherSession(ConnectionProperties connectionProperties)
            : this(connectionProperties, new WebClient(connectionProperties.BuildBasicAuthCredentials()))
        {
        }

        internal CypherSession(ConnectionProperties connectionProperties, IWebClient webClient)
        {
            _uri = connectionProperties.Url;
            _webClient = webClient;
            _entityCache = new DictionaryEntityCache();
            _webSerializer = new DefaultJsonSerializer(_entityCache);
        }

        internal void Connect()
        {
            IHttpResponseMessage response = null;
            try
            {
                var getTask = _webClient.GetAsync(_uri);
                getTask.Wait();
                response = getTask.Result;
            }
            catch (Exception)
            {
                throw new NeoServerUnavalaibleExpcetion(_uri);
            }
            var readTask = response.Content.ReadAsStringAsync();
            Task.WaitAll(readTask);
            var json = readTask.Result;
            var serviceResponse = _webSerializer.Deserialize<ServiceRootResponse>(json);

            AssertVersion(serviceResponse);
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
            return new FluentCypherQueryBuilder<TVariables>(new CypherClientFactory(_uri, _webClient, _webSerializer, _entityCache));
        }

        public ICypherQueryStart<TVariables> BeginQuery<TVariables>(
            Expression<Func<ICypherPrototype, TVariables>> variablePrototype)
        {
            return new FluentCypherQueryBuilder<TVariables>(new CypherClientFactory(_uri, _webClient, _webSerializer, _entityCache));
        }

        public Node CreateNode(object properties)
        {
            return CreateNode(properties, null);
        }

        public Node CreateNode(object properties, params string[] labels)
        {
            var props = _webSerializer.Serialize(properties);
            var propNames = new EntityReturnColumns(NodeVariableName);

            var clause = String.Format(
                CreateNodeClauseFormat,
                labels != null && labels.Any() ? ":" + string.Join(":", labels) : string.Empty,
                props,
                propNames.PropertiesPropertyName,
                propNames.IdPropertyName,
                propNames.LabelsPropertyName);
            var endpoint = new CypherClientFactory(_uri, _webClient, _webSerializer, _entityCache).Create();
            var result = endpoint.ExecuteQuery<SingleNodeResult>(clause);
            var node = result.First().NewNode;
            return node;
        }


        public Relationship CreateRelationship(Node node1, Node node2, string type, object relationshipProperties = null)
        {
            var query = BeginQuery(n => new {node1 = n.Node, node2 = n.Node, rel = n.Rel})
                .Start(ctx => ctx
                                  .StartAtId(ctx.Vars.node1, node1.Id)
                                  .StartAtId(ctx.Vars.node2, node2.Id))
                .Create(ctx => ctx.CreateRel(ctx.Vars.node1, ctx.Vars.rel, type, relationshipProperties, ctx.Vars.node2))
                .Return(ctx => ctx.Vars.rel)
                .Fetch();

            return query.FirstOrDefault();
        }

        public void DeleteRelationship(long relationshipId)
        {
            BeginQuery(n => new { relationship = n.Rel }).Start(v => v.StartAtId(v.Vars.relationship, relationshipId)).Delete(v => v.relationship).Execute();
            _entityCache.Remove<Relationship>(relationshipId);
        }

        public void DeleteRelationship(Relationship relationship)
        {
            DeleteRelationship(relationship.Id);
        }

        public Node GetNode(long id)
        {
            if (_entityCache.Contains<Node>(id))
            {
                return _entityCache.GetEntity<Node>(id);
            }
            else
            {
                var query = BeginQuery(n => new {newNode = n.Node})
                    .Start(v => v.StartAtId(v.Vars.newNode, id))
                    .Return(ctx => new {ctx.Vars.newNode})
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
            _entityCache.Remove<Node>(nodeId);
        }

        public void Delete(Node node)
        {
            Delete(node.Id);
        }

        public void Clear()
        {
            _entityCache.Clear();
        }

        public void CreateConstraint(string label, string property)
        {
            var clause = string.Format(CreateConstraintClauseFormat, NodeVariableName, label, property);
            var endpoint = new CypherClientFactory(_uri, _webClient, _webSerializer, _entityCache).Create();
            endpoint.ExecuteCommand(clause);
        }

        public void DropConstraint(string label, string property)
        {
            var clause = string.Format(DropConstraintClauseFormat, NodeVariableName, label, property);
            var endpoint = new CypherClientFactory(_uri, _webClient, _webSerializer, _entityCache).Create();
            endpoint.ExecuteCommand(clause);
        }

        public void CreateIndex(string label, string property)
        {
            var clause = string.Format(CreateIndexClauseFormat, label, property);
            var endpoint = new CypherClientFactory(_uri, _webClient, _webSerializer, _entityCache).Create();
            endpoint.ExecuteCommand(clause);
        }

        public void DropIndex(string label, string property)
        {
            var clause = string.Format(DropIndexClauseFormat, label, property);
            var endpoint = new CypherClientFactory(_uri, _webClient, _webSerializer, _entityCache).Create();
            endpoint.ExecuteCommand(clause);
        }

        private static readonly MethodInfo SetMethodInfo = typeof(IUpdateQueryContext<SingleNodeResult>).GetMethod("Set");
        private static readonly PropertyInfo VarsProperty =
                (PropertyInfo)ReflectOn<IUpdateQueryContext<SingleNodeResult>>.Member(c => c.Vars).MemberInfo;
        private static readonly PropertyInfo NewNodeProperty =
            (PropertyInfo) ReflectOn<SingleNodeResult>.Member(c => c.NewNode).MemberInfo;

        public void Save(Node node)
        {
            var props = node as IDynamicMetaData;
            var vals = props.GetAllValues().Where(kvp => !Node.NodePropertyNames.Contains(kvp.Key));
            
            var setActions = new List<Expression<Func<IUpdateQueryContext<SingleNodeResult>, ISetResult>>>();
            foreach (var val in vals)
            {
                var param = Expression.Parameter(typeof(IUpdateQueryContext<SingleNodeResult>));
                var propType = val.Value.GetType();
                var method = SetMethodInfo.MakeGenericMethod(new[] { typeof(Node), propType });
                var member = Expression.Property(Expression.Property(param, VarsProperty), NewNodeProperty);
                var call = Expression.Call(param, method, member, Expression.Constant(val.Key), Expression.Constant(val.Value));
                var lambda = Expression.Lambda<Func<IUpdateQueryContext<SingleNodeResult>, ISetResult>>(call, param);
                setActions.Add(lambda);
            }

            BeginQuery<SingleNodeResult>().Start(v => v.StartAtId(v.Vars.NewNode, node.Id))
                                          .Update(setActions.ToArray())
                                          .Execute();
        }

        #endregion

        internal class SingleNodeResult
        {
            public SingleNodeResult(Node newNode)
            {
                NewNode = newNode;
            }

            public Node NewNode { get; private set; }
        }
    }

    internal static class ConnectionPropertiesExtension
    {
        public static BasicAuthCredentials BuildBasicAuthCredentials(this ConnectionProperties connectionProperties)
        {
            if (!(new[] {connectionProperties.Username, connectionProperties.Password}.All(string.IsNullOrEmpty)))
            {
                return new BasicAuthCredentials(connectionProperties.Username, connectionProperties.Password);
            }

            return null;
        }
    }
}