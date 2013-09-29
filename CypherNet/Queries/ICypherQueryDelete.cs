using System;
using System.Linq.Expressions;

namespace CypherNet.Queries
{
    public interface ICypherQueryDelete<TVariables>
    {
        ICypherExecuteable Delete<TOut>(Expression<Func<TVariables, TOut>> func);
    }
}