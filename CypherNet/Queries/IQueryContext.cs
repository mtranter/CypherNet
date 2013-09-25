namespace CypherNet.Queries
{
    public interface IQueryContext<out TVariables>
    {
        TVariables Vars { get; }
    }
}