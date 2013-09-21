namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryReturns<TVariables> : ICypherQueryCreate<TVariables>, ICypherQueryDelete<TVariables>
    {
        ICypherOrderBy<TVariables, TOut> Return<TOut>(Expression<Func<TVariables, TOut>> func);
    }

    public interface ICypherQueryReturnOrExecute<TVariables> : ICypherExecuteable
    {
        ICypherOrderBy<TVariables, TOut> Return<TOut>(Expression<Func<TVariables, TOut>> func);
    }

    public interface ICypherQueryDelete<TVariables>
    {
        ICypherExecuteable Delete<TOut>(Expression<Func<TVariables, TOut>> func);
    }

    public interface ICypherQueryReturnOnly<TVariables>
    {
        ICypherFetchable<TVariables> Return(Expression<Func<TVariables, TVariables>> func);
    }
}