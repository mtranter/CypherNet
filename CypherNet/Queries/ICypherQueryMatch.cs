namespace CypherNet.Queries
{
    using System;
    using System.Linq.Expressions;

    public interface ICypherQueryMatch<TVariables> : ICypherQueryMatchOnly<TVariables>, ICypherQueryWhere<TVariables>, ICypherQueryCreate<TVariables>
    {
    }

    public interface ICypherQuerySetable<TVariables> : ICypherQueryCreate<TVariables>
    {
        ICypherQueryReturns<TVariables> Update(params Expression<Action<TVariables>>[] setters);
    }

    public interface ICypherQueryCreate<TVariables>
    {
        ICypherQueryReturns<TVariables> Create(Expression<Func<TVariables, ICreateCypherRelationship>> matchDef);
    }

    public interface ICreateCypherRelationship
    {
    }
}