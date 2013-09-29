using CypherNet.Serialization;

namespace CypherNet.Transaction
{
    #region

    using Http;

    #endregion

    internal class DefaultCypherSessionFactory : ICypherSessionFactory
    {
        private readonly string _sourceUri;

        public DefaultCypherSessionFactory(string sourceUri)
        {
            _sourceUri = sourceUri;
        }

        public ICypherSession Create()
        {
            return Create(_sourceUri);
        }

        public ICypherSession Create(string sourceUri)
        {
            var session = new CypherSession(sourceUri);
            session.Connect();
            return session;
        }
    }
}