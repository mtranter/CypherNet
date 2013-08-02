namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryReturns<TVariables>
    {
        ICypherOrderBy<TVariables,TOut> Return<TOut>(Expression<Func<TVariables, TOut>> func);
    }
}