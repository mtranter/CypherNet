namespace CypherNet.Serialization
{
    #region

    using System.Reflection;
    using Graph;

    #endregion

    internal class EntityReturnColumns
    {
        private readonly PropertyInfo _cypherVariableProperty;
        private readonly string _propertyName;

        public EntityReturnColumns(PropertyInfo cypherVariableProperty)
            : this(cypherVariableProperty.Name)
        {
            _cypherVariableProperty = cypherVariableProperty;
        }

        public EntityReturnColumns(string propertyName)
        {
            _propertyName = propertyName;
        }

        public bool RequiresTypeProperty
        {
            get { return _cypherVariableProperty != null && _cypherVariableProperty.PropertyType == typeof (Relationship); }
        }

        public bool RequiresLabelsProperty
        {
            get { return _cypherVariableProperty != null && _cypherVariableProperty.PropertyType == typeof (Node); }
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