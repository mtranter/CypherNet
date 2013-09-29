using System.Collections.Generic;

namespace CypherNet
{
    #region

    

    #endregion

    public interface ICypherClient
    {
        IEnumerable<TOut> ExecuteQuery<TOut>(string cypherQuery);
        void ExecuteCommand(string cypherCommand);
    }
}