using System;
using System.Collections.Generic;
using System.Data.Common;
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
        private readonly ConnectionProperties _connectionProperties;

        public SessionConfiguration(string connectionString)
        {
            _connectionProperties = Neo4JConnectionStringParser.Parse(connectionString);
            Logging.Logger.Current = new TraceLogger();
        }

        public ICypherSessionFactory CreateSessionFactory()
        {
            return new DefaultCypherSessionFactory(_connectionProperties);
        }
    }

    public interface ISessionConfiguration
    {
        ICypherSessionFactory CreateSessionFactory();
    }

    public class ConnectionProperties
    {
        public ConnectionProperties(string url): this(url, null, null)
        {
        }

        public ConnectionProperties(string url, string username, string password)
        {
            Url = url;
            Username = username;
            Password = password;
        }

        public string Url { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }
    }
}