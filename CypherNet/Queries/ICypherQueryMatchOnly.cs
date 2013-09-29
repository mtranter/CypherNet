namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryMatchOnly<TVariables>
    {
        ICypherQueryWhere<TVariables> Match(
            params Expression<Func<IMatchQueryContext<TVariables>, IDefineCypherRelationship>>[] matchDef);
    }
}