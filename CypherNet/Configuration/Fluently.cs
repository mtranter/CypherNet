
using CypherNet.Http;
using CypherNet.Serialization;
using CypherNet.Transaction;

namespace CypherNet.Configuration
{
    public class Fluently
    {
        public static ISessionConfiguration Configure(string endpointUri)
        {
            return new SessionConfiguration(endpointUri);
        }
    }

    class SessionConfiguration : ISessionConfiguration
    {
        private readonly string _endpointUri;

        public SessionConfiguration(string endpointUri)
        {
            _endpointUri = endpointUri;
        }

        public ICypherSessionFactory CreateSessionFactory()
        {
            return new DefaultCypherSessionFactory(_endpointUri, new WebClient(new DefaultJsonSerializer()));
        }
    }

    public interface ISessionConfiguration
    {
        ICypherSessionFactory CreateSessionFactory();
    }
}
