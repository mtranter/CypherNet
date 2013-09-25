using System;
using System.Linq.Expressions;

namespace CypherNet.Queries
{
    public interface ICypherQueryReturnOrExecute<TVariables> : ICypherExecuteable
    {
        ICypherOrderBy<TVariables, TOut> Return<TOut>(Expression<Func<TVariables, TOut>> func);
    }
}