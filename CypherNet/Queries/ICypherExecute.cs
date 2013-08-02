using System;
using System.Linq.Expressions;

namespace CypherNet.Queries
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface ICypherExecuteable<out TResult>
    {
        IEnumerable<TResult> Execute();
    }

    public interface ICypherOrderBy<TParams, out TResult> :  ICypherExecuteable<TResult>
    {
        ICypherExecuteable<TParams> OrderBy(params Expression<Func<TParams, dynamic>>[] orderBy);
    }
}