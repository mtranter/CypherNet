using System;

namespace CypherNet.Transaction
{
    internal class NeoServerUnavalaibleExpcetion : Exception
    {
        public NeoServerUnavalaibleExpcetion(string uri)
            : base("Cannot acces Neo4j server at " + uri)
        {
        }

        public NeoServerUnavalaibleExpcetion(string uri, string statusCode)
            : base(string.Format("Cannot acces Neo4j server at {0}.  Status Code is:{1}", uri, statusCode))
        {
        }
    }
}