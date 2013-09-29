namespace CypherNet
{
    public interface ICypherSessionFactory
    {
        ICypherSession Create();
        ICypherSession Create(string sourceUri);
    }
}