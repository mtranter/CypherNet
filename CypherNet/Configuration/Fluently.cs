namespace CypherNet.Configuration
{
    #region

    using Http;
    using Serialization;
    using Transaction;

    #endregion

    public class Fluently
    {
        public static ISessionConfiguration Configure(string endpointUri)
        {
            return new SessionConfiguration(endpointUri);
        }
    }

    internal class SessionConfiguration : ISessionConfiguration
    {
        private readonly string _endpointUri;

        public SessionConfiguration(string endpointUri)
        {
            _endpointUri = endpointUri;
        }

        public ICypherSessionFactory CreateSessionFactory()
        {
            var webClient = new WebClient(new DefaultJsonSerializer());
            return new DefaultCypherSessionFactory(_endpointUri, webClient);
        }
    }

    public interface ISessionConfiguration
    {
        ICypherSessionFactory CreateSessionFactory();
    }
}