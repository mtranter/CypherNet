using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CypherNet.Transaction
{
    public class CypherResponseException : Exception
    {
        public CypherResponseException(string[] errors)
            : base("Neo4j Server has returned one or more exceptions: " + String.Join(Environment.NewLine, errors))
        {
        }
    }
}
