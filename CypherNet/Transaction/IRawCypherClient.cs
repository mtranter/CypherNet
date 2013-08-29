namespace CypherNet.Transaction
{
    internal interface IRawCypherClient 
    {
        TResult Run<TResult>(string cypher);
    }
}