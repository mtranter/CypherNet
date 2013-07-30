namespace CypherNet.Queries
{
    public interface ICypherQueryMatch<TVariables> : ICypherQueryMatchOnly<TVariables>, ICypherQueryWhere<TVariables>
    {
    }
}