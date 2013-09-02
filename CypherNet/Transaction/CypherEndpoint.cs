using CypherNet.Graph;

namespace CypherNet.Transaction
{
    using System;
    using Queries;

    class CypherEndpoint : ICypherEndpoint
    {
        protected IRawCypherClient CypherClient {get; private set;}

        internal CypherEndpoint(IRawCypherClient requestBuilder)
        {
            CypherClient = requestBuilder;
        }

        #region ICypherEndpoint Members

        public ICypherQueryStart<TVariables> BeginQuery<TVariables>()
        {
            return new FluentCypherQueryBuilder<TVariables>(CypherClient);
        }

        public ICypherQueryStart<TVariables> BeginQuery<TVariables>(System.Linq.Expressions.Expression<Func<TVariables>> variablePrototype)
        {
            return new FluentCypherQueryBuilder<TVariables>(CypherClient);
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