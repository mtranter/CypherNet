namespace CypherNet.Serialization
{
    #region

    using System;

    #endregion

    public class CypherColumnNotPresentException : Exception
    {
        public CypherColumnNotPresentException(string columnName)
            : base("Expected column: " + columnName + " as part of the cypher result set")
        {
        }
    }
}