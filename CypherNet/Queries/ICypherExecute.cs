using System;
using System.Linq.Expressions;

namespace CypherNet.Queries
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface ICypherFetchable<out TResult> : ICypherExecuteable
    {
        IEnumerable<TResult> Fetch();
    }

    public interface ICypherExecuteable
    {
        void Execute();
    }

    public interface ICypherOrderBy<TParams, out TResult> :  ICypherSkip<TParams, TResult>
    {
        ICypherSkip<TParams, TResult> OrderBy(params Expression<Func<TParams, dynamic>>[] orderBy);
    }

    public interface ICypherSkip<TParams, out TResult> : ICypherLimit<TParams, TResult>
    {
        ICypherLimit<TParams, TResult> Skip(int skip);
    }

    public interface ICypherLimit<TParams, out TResult> : ICypherFetchable<TResult>
    {
        ICypherFetchable<TResult> Limit(int limit);
    }
}