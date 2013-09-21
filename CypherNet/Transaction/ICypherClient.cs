namespace CypherNet.Transaction
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface ICypherClient
    {
        IEnumerable<TOut> ExecuteQuery<TOut>(string cypherQuery);
        void ExecuteCommand(string cypherCommand);
    }
}