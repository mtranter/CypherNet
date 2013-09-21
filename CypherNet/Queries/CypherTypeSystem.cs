namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    internal class CypherTypeSystem
    {
        private static readonly List<Type> ValidVariableTypes = new List<Type>();

        public static void AssertTypePopertiesAreOneOfAny<TVariables>(params Type[] validTypes)
        {
            AssertTypePopertiesAreOneOfAny(typeof (TVariables));
        }

        public static void AssertTypePopertiesAreOneOfAny(Type sourceType, params Type[] validTypes)
        {
            if (ValidVariableTypes.Contains(sourceType))
            {
                return;
            }

            if (!IsValidVaraibleType(sourceType, validTypes))
            {
                throw new InvalidCypherVariableTypeException(sourceType);
            }

            ValidVariableTypes.Add(sourceType);
        }

        private static bool IsValidVaraibleType(Type variableType, IEnumerable<Type> validTypes)
        {
            return variableType.GetProperties().All(p => validTypes.Contains(p.PropertyType));
        }
    }
}