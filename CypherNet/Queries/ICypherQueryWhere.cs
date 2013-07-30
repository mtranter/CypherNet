namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryWhere<TVariables> : ICypherQueryReturns<TVariables>
    {
        ICypherQueryReturns<TVariables> Where();
        ICypherQueryReturns<TVariables> Where(Expression<Func<TVariables, bool>> predicate);
    }
}