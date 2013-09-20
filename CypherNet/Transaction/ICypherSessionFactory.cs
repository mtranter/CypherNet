namespace CypherNet.Transaction
{
    public interface ICypherSessionFactory
    {
        ICypherSession Create();
        ICypherSession Create(string sourceUri);
    }
}