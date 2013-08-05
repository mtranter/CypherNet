namespace CypherNet.Queries
{
    using System;
    using System.Linq.Expressions;

    public interface ICypherQueryMatch<TVariables> : ICypherQueryMatchOnly<TVariables>, ICypherQueryWhere<TVariables>
    {
    }

    public interface ICypherQuerySetable<TVariables>
    {
        ICypherQueryReturns<TVariables> Update(params Expression<Action<TVariables>>[] setters);
    }

    public interface ICypherQueryCreate<TVariables>
    {
        ICypherQueryWhere<TVariables> Match(Expression<Func<TVariables, ICreateCypherRelationship>> matchDef);
    }
}