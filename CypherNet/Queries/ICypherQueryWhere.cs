namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryWhere<TVariables> : ICypherQueryReturns<TVariables>, ICypherQuerySetable<TVariables>,
                                                     ICypherQueryCreate<TVariables>
    {
        ICypherQueryReturns<TVariables> Where(Expression<Func<IWhereQueryContext<TVariables>, bool>> predicate);
    }
}