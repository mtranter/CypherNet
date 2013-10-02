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
            Logging.Logger.Current = new TraceLogger();
        }

        public ICypherSessionFactory CreateSessionFactory()
        {
            return new DefaultCypherSessionFactory(_endpointUri);
        }
    }

    public interface ISessionConfiguration
    {
        ICypherSessionFactory CreateSessionFactory();
    }
}