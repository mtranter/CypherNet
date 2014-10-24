namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryOptionalMatchOnly<TVariables> : ICypherQueryWhere<TVariables>
    {
        ICypherQueryWhere<TVariables> OptionalMatch(
            params Expression<Func<IMatchQueryContext<TVariables>, IDefineCypherRelationship>>[] matchDef);
    }
}