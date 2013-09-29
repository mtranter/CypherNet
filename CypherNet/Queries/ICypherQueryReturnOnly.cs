using System;
using System.Linq.Expressions;

namespace CypherNet.Queries
{
    public interface ICypherQueryReturnOnly<TVariables>
    {
        ICypherFetchable<TVariables> Return(Expression<Func<TVariables, TVariables>> func);
    }
}