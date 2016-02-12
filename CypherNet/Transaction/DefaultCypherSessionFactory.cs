

namespace CypherNet.Transaction
{
    #region

    using Http;
    using CypherNet.Configuration;
    using CypherNet.Serialization;

    #endregion

    internal class DefaultCypherSessionFactory : ICypherSessionFactory
    {
        private readonly ConnectionProperties _connectionProperties;

        public DefaultCypherSessionFactory(ConnectionProperties connectionProperties)
        {
            _connectionProperties = connectionProperties;
        }

        public ICypherSession Create()
        {
            return Create(_connectionProperties);
        }

        public ICypherSession Create(ConnectionProperties connectionProperties)
        {
            var session = new CypherSession(connectionProperties);
            session.Connect();
            return session;
        }
    }
}