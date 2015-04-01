using CypherNet.Serialization;

namespace CypherNet.Transaction
{
    #region

    using Http;

    #endregion

    internal class DefaultCypherSessionFactory : ICypherSessionFactory
    {
        private readonly string _sourceUri;
        private readonly string _username;
        private readonly string _password;

        public DefaultCypherSessionFactory(string sourceUri, string username, string password)
        {
            _sourceUri = sourceUri;
            _username = username;
            _password = password;
        }

        public ICypherSession Create()
        {
            return Create(_sourceUri);
        }

        public ICypherSession Create(string sourceUri)
        {
            var session = new CypherSession(sourceUri, _username, _password);
            session.Connect();
            return session;
        }
    }
}