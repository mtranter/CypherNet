using System;
using System.Linq.Expressions;

namespace CypherNet.Queries
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface ICypherExecute<out TResult>
    {
        IEnumerable<TResult> Execute();
    }

    public interface ICypherOrderBy<TParams>
    {
        IEnumerable<TParams> OrderBy(params Expression<Func<TParams>>[] orderBy);
    }
}