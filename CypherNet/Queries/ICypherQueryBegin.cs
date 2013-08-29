namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;

    #endregion

    public interface ICypherQueryBegin
    {
        ICypherQueryStart<TVariables> QueryUsing<TVariables>();
        ICypherQueryStart<TVariables> QueryUsing<TVariables>(Expression<Func<TVariables>> func);
    }
}