namespace CypherNet.Transaction
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
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
            throw new NotImplementedException();
        }

        public ICypherQueryStart<TVariables> BeginQuery<TVariables>(System.Linq.Expressions.Expression<Func<TVariables>> variablePrototype)
        {
            throw new NotImplementedException();
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