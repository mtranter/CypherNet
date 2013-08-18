namespace CypherNet.Queries
{
    #region

    

    #endregion

    internal interface ICypherQueryBuilder
    {
        string BuildQueryString<TIn, TOut>(CypherQueryDefinition<TIn, TOut> queryDefinition);
    }
}