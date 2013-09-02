using CypherNet.Graph;

namespace CypherNet.Transaction
{
    using System;
    using Queries;

    public class CypherEndpoint : ICypherEndpoint
    {
        private readonly ICypherClientFactory _clientFactory;

        internal CypherEndpoint(ICypherClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
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

        public ICypherQueryReturnOnly<Graph.Node> CreateNode(object properties)
        {
            throw new NotImplementedException();
        }

        public ICypherQueryReturnOnly<Graph.Node> CreateNode(object properties, string label)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}