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

    public interface ICypherQueryDelete<TVariables>
    {
        ICypherFetchable<TOut> Delete<TOut>(Expression<Func<TVariables, TOut>> func);
    }

    public interface ICypherQueryReturnOnly<TVariables>
    {
        ICypherFetchable<TVariables> Return(Expression<Func<TVariables, TVariables>> func);
    }
}