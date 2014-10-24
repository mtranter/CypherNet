namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryMatch<TVariables> : ICypherQueryMatchOnly<TVariables>, ICypherQueryOptionalMatchOnly<TVariables>
    {
    }

    public interface ICypherQuerySetable<TVariables> : ICypherQueryCreate<TVariables>
    {
        ICypherQueryReturnOrExecute<TVariables> Update(params Expression<Func<IUpdateQueryContext<TVariables>, ISetResult>>[] setters);
    }

    public interface ICypherQueryCreate<TVariables>
    {
        ICypherQueryReturns<TVariables> Create(Expression<Func<ICreateRelationshipQueryContext<TVariables>, ICreateCypherRelationship>> matchDef);
    }

    public interface ICreateCypherRelationship
    {
    }
}