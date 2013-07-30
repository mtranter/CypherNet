using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CypherNet.Serialization
{
    public class CypherColumnNotPresentException : Exception
    {
        public CypherColumnNotPresentException(string columnName)
            : base("Expected column: " + columnName + " as part of the cypher result set")
        {
            
        }
    }
}
