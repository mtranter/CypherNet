using System;
using CypherNet.Logging;

namespace CypherNet.Configuration
{
    #region

    using Http;
    using Serialization;
    using Transaction;

    #endregion

    public class Fluently
    {
        public static ISessionConfiguration Configure(string endpointUri, string username, string password)
        {
            return new SessionConfiguration(endpointUri, username, password);
        }
    }

    internal class SessionConfiguration : ISessionConfiguration
    {
        private readonly string _endpointUri;
        private readonly string _username;
        private readonly string _password;

        public SessionConfiguration(string endpointUri, string username, string password)
        {
            _endpointUri = endpointUri;
            _username = username;
            _password = password;
            Logging.Logger.Current = new TraceLogger();
        }

        public ICypherSessionFactory CreateSessionFactory()
        {
            return new DefaultCypherSessionFactory(_endpointUri, _username, _password);
        }
    }

    public interface ISessionConfiguration
    {
        ICypherSessionFactory CreateSessionFactory();
    }
}