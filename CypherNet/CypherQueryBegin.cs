namespace CypherNet
{
    using System;
    using System.Collections.Generic;
    using Queries;

    internal class CypherQueryBegin : ICypherQueryBegin
    {
        private readonly ICypher _cypher;

        public CypherQueryBegin(ICypher cypher)
        {
            _cypher = cypher;
        }

        #region ICypherQueryBegin Members

        public ICypherQueryStart<TVariables> QueryUsing<TVariables>(System.Linq.Expressions.Expression<Func<TVariables>> func)
        {
            return new FluentCypherQueryBuilder<TVariables>(_cypher, new TransactionEndpointCypherQueryBuilder());
        }

        #endregion
    }
}
