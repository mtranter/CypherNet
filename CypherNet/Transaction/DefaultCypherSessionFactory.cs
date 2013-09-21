namespace CypherNet.Transaction
{
    #region

    using Http;

    #endregion

    internal class DefaultCypherSessionFactory : ICypherSessionFactory
    {
        private readonly IWebClient _client;
        private readonly string _sourceUri;

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