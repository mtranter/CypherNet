using CypherNet.Serialization;

namespace CypherNet.Transaction
{
    #region

    using Http;

    #endregion

    internal class DefaultCypherSessionFactory : ICypherSessionFactory
    {
        private readonly IWebClient _client;
        private readonly string _sourceUri;

        public DefaultCypherSessionFactory(string sourceUri)
        {
            _sourceUri = sourceUri;
        }

        public ICypherSession Create()
        {
            return new CypherSession(_sourceUri);
        }

        public ICypherSession Create(string sourceUri)
        {
            return new CypherSession(sourceUri);
        }
    }
}