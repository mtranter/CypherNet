namespace CypherNet.Serialization
{
    #region

    using System;

    #endregion

    [AttributeUsage(AttributeTargets.Constructor)]
    internal class DeserializeUsingAttribute : Attribute
    {
    }
}