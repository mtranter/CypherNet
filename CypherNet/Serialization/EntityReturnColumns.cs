namespace CypherNet.Serialization
{
    #region

    using System.Reflection;
    using Graph;

    #endregion

    internal class EntityReturnColumns
    {
        private readonly string _propertyName;
        
        public EntityReturnColumns(string propertyName)
        {
            _propertyName = propertyName;
        }

        public string PropertiesPropertyName
        {
            get { return _propertyName; }
        }

        public string IdPropertyName
        {
            get { return _propertyName + "__Id"; }
        }

        public string TypePropertyName
        {
            get { return _propertyName + "__Type"; }
        }

        public string LabelsPropertyName
        {
            get { return _propertyName + "__Labels"; }
        }
    }
}