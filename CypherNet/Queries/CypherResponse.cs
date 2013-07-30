namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections.Generic;

    #endregion

    internal interface ICypherResponse
    {
        Type ResponseType { get; }
    }

    internal class CypherResponse<TResult> : ICypherResponse
    {
        internal CypherResponse(IEnumerable<TResult> resultSet)
        {
            Results = resultSet;
        }

        internal IEnumerable<TResult> Results { get; private set; }

        public Type ResponseType
        {
            get { return typeof (TResult); }
        }
    }
}