#region



#endregion

namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq;
    using Graph;

    #endregion

    public class InvalidCypherVariableTypeException : Exception
    {
        private const string ErrorMessageFormat =
            "Invalid type provided as a Cypher Variable definition. All properties must be one of the following types: {0}";

        public InvalidCypherVariableTypeException(Type invalidType)
            : base(BuildMessage(invalidType))
        {
        }

        private static string BuildMessage(Type type)
        {
            var validTypeNames = String.Join(", ", new[] { typeof(IGraphEntity) }.Select(t => t.Name));
            return String.Format(ErrorMessageFormat, validTypeNames);
        }
    }
}