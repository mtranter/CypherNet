
namespace CypherNet.Queries
{
    #region

    using System;

    #endregion

    [AttributeUsage(AttributeTargets.Interface)]
    internal class CypherGraphEntityNameAttribute : Attribute
    {
        public CypherGraphEntityNameAttribute(string entityType)
        {
            EntityType = entityType;
        }

        public string EntityType { get; private set; }
    }
}