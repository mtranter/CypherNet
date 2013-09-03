using CypherNet.Graph;
using CypherNet.Serialization;
using StaticReflection;

namespace CypherNet.Transaction
{
    using System;
    using Queries;
    using System.Linq;

    public class CypherEndpoint : ICypherEndpoint
    {
        private readonly ICypherClientFactory _clientFactory;
        private readonly IWebSerializer _webSerializer;
        private static readonly string NodeVariableName = ReflectOn<CreateNodeResult>.Member(a => a.NewNode).Name;
        private static readonly string CreateNodeClauseFormat = String.Format(@"CREATE ({0}{{0}} {{1}}) RETURN {0} as {{2}}, id({0}) as {{3}};", NodeVariableName);

        internal CypherEndpoint(ICypherClientFactory clientFactory)
            : this(clientFactory, new DefaultJsonSerializer())
        {
        }

        internal CypherEndpoint(ICypherClientFactory clientFactory, IWebSerializer webSerializer)
        {
            _clientFactory = clientFactory;
            _webSerializer = webSerializer;
        }

        #region ICypherEndpoint Members

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
                                       propNames.PropertiesPropertyName, propNames.IdPropertyName);
            var endpoint = _clientFactory.Create();
            var result = endpoint.ExecuteQuery<CreateNodeResult>(clause);
            return result.First().NewNode;
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