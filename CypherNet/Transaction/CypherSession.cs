

namespace CypherNet.Transaction
{
    using System;
    using Queries;
    using System.Linq;
    using Graph;
    using Serialization;
    using StaticReflection;

    public class CypherSession : ICypherSession
    {
        private readonly ICypherClientFactory _clientFactory;
        private readonly IWebSerializer _webSerializer;
        private static readonly string NodeVariableName = ReflectOn<CreateNodeResult>.Member(a => a.NewNode).Name;
        private static readonly string CreateNodeClauseFormat = String.Format(@"CREATE ({0}{{0}} {{1}}) RETURN {0} as {{2}}, id({0}) as {{3}}, labels({0}) as {{4}};", NodeVariableName);

        internal CypherSession(ICypherClientFactory clientFactory)
            : this(clientFactory, new DefaultJsonSerializer())
        {
        }

        internal CypherSession(ICypherClientFactory clientFactory, IWebSerializer webSerializer)
        {
            _clientFactory = clientFactory;
            _webSerializer = webSerializer;
        }

        #region ICypherSession Members

        public ICypherQueryStart<TVariables> BeginQuery<TVariables>()
        {
            return new FluentCypherQueryBuilder<TVariables>(_clientFactory);
        }

        public ICypherQueryStart<TVariables> BeginQuery<TVariables>(System.Linq.Expressions.Expression<Func<ICypherPrototype,TVariables>> variablePrototype)
        {
            return new FluentCypherQueryBuilder<TVariables>(_clientFactory);
        }

        public Graph.Node CreateNode(object properties)
        {
            return CreateNode(properties, null);
        }
        
        public Node CreateNode(object properties, string label)
        {
            var props = _webSerializer.Serialize(properties);
            var propNames = new EntityReturnColumns(NodeVariableName);
            var clause = String.Format(CreateNodeClauseFormat, String.IsNullOrEmpty(label) ? "" : ":" + label, props,
                                       propNames.PropertiesPropertyName, propNames.IdPropertyName, propNames.LabelsPropertyName);
            var endpoint = _clientFactory.Create();
            var result = endpoint.ExecuteQuery<CreateNodeResult>(clause);
            return result.First().NewNode;
        }

        public Node GetNode(long id)
        {
            var query = BeginQuery(n => new {newNode = n.Node})
                .Start(v => Start.At(v.newNode, id))
                .Return(v => new {v.newNode})
                .Fetch();
            var firstRow = query.FirstOrDefault();
            return firstRow == null ? null : firstRow.newNode;
        }

        public void DeleteNode(long nodeId)
        {
            throw new NotImplementedException();
        }

        public void UpdateNode(long nodeId, object properties)
        {
            throw new NotImplementedException();
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
    }
}