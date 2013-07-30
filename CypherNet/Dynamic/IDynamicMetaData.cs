#region



#endregion

namespace CypherNet.Dynamic
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface IDynamicMetaData
    {
        IEnumerable<string> AllPropertyNames { get; }
        bool HasProperty(string propertyName);
        IEnumerable<KeyValuePair<string, object>> GetAllValues();
    }
}