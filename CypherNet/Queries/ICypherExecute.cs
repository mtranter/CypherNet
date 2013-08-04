using System;
using System.Linq.Expressions;

namespace CypherNet.Queries
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface ICypherExecuteable<TIn, out TResult>
    {
        IEnumerable<TResult> Execute();
    }

    public interface ICypherOrderBy<TParams, out TResult> :  ICypherSkip<TParams, TResult>
    {
        ICypherSkip<TParams, TResult> OrderBy(params Expression<Func<TParams, dynamic>>[] orderBy);
    }

    public interface ICypherSkip<TParams, out TResult> : ICypherLimit<TParams, TResult>
    {
        ICypherLimit<TParams, TResult> Skip(int skip);
    }

    public interface ICypherLimit<TParams, out TResult> : ICypherExecuteable<TParams, TResult>
    {
        ICypherExecuteable<TParams, TResult> Limit(int limit);
    }
}