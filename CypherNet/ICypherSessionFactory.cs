using CypherNet.Configuration;

namespace CypherNet
{
    public interface ICypherSessionFactory
    {
        ICypherSession Create();
        ICypherSession Create(ConnectionProperties connectionProperties);
    }
}