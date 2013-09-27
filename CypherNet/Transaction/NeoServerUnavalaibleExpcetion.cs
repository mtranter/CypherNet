using System;

namespace CypherNet.Transaction
{
    internal class NeoServerUnavalaibleExpcetion : Exception
    {
        public NeoServerUnavalaibleExpcetion(string uri)
            : base("Cannot acces Neo4j server at " + uri)
        {
        }
    }
}