using CypherNet.Http;

namespace CypherNet.Transaction
{
    class DefaultCypherSessionFactory : ICypherSessionFactory
    {
        private readonly string _sourceUri;
        private readonly IWebClient _client;

        public DefaultCypherSessionFactory(string sourceUri, IWebClient client)
        {
            _sourceUri = sourceUri;
            _client = client;
        }

        public ICypherSession Create()
        {
            return new CypherSession(new CypherClientFactory(_sourceUri, _client));
        }

        public ICypherSession Create(string sourceUri)
        {
            return new CypherSession(new CypherClientFactory(sourceUri, _client));
        }
    }
}