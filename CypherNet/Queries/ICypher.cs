namespace CypherNet.Queries
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface ICypher
    {
        IEnumerable<TOut> ExecuteQuery<TOut>(string cypherQuery);
        void ExecuteCommand(string cypherCommand);
    }
}