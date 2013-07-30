namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryBegin
    {
        ICypherQueryStart<TVariables> Variables<TVariables>(Expression<Func<TVariables>> func);
    }
}