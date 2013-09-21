namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryMatch<TVariables> : ICypherQueryMatchOnly<TVariables>, ICypherQueryWhere<TVariables>,
                                                     ICypherQueryCreate<TVariables>
    {
    }

    public interface ICypherQuerySetable<TVariables> : ICypherQueryCreate<TVariables>
    {
        ICypherQueryReturnOrExecute<TVariables> Update(params Expression<Action<TVariables>>[] setters);
    }

    public interface ICypherQueryCreate<TVariables>
    {
        ICypherQueryReturns<TVariables> Create(Expression<Func<TVariables, ICreateCypherRelationship>> matchDef);
    }

    public interface ICreateCypherRelationship
    {
    }
}