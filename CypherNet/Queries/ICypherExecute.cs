namespace CypherNet.Queries
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface ICypherExecute<out TResult>
    {
        IEnumerable<TResult> Execute();
    }
}