namespace CypherNet.Transaction
{
    using System.Collections.Generic;
    using Queries;

    public interface IRawCypherClient 
    {
        IEnumerable<TOut> ExecuteQuery<TOut>(string cypherQuery);
        void ExecuteCommand(string cypherCommand);
    }
}