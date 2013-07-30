namespace CypherNet.Queries
{
    #region

    using System.Collections.Generic;

    #endregion

    internal class CypherQueryExecute<TOut> : ICypherExecute<TOut>
    {
        private readonly ICypher _cypherEndpoint;
        private readonly string _query;

        internal CypherQueryExecute(ICypher cypherEndpoint, string query)
        {
            _cypherEndpoint = cypherEndpoint;
            _query = query;
        }

        #region ICypherExecute<TOut> Members

        public IEnumerable<TOut> Execute()
        {
            return _cypherEndpoint.ExecuteQuery<TOut>(_query);
        }

        #endregion
    }
}