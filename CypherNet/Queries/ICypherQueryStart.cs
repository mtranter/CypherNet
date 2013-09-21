namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryStart<TVariables> : ICypherQueryMatchOnly<TVariables>
    {
        ICypherQueryMatch<TVariables> Start(Expression<Action<TVariables>> startDef);
    }
}