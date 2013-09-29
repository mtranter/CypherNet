namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryReturns<TVariables> : ICypherQueryCreate<TVariables>, ICypherQueryDelete<TVariables>
    {
        ICypherOrderBy<TVariables, TOut> Return<TOut>(Expression<Func<IReturnQueryContext<TVariables>, TOut>> func);
    }
}