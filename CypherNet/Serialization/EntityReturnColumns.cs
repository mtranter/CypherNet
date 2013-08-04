using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherNet.Serialization
{
    using System.Reflection;
    using Graph;

    class EntityReturnColumns
    {
        private readonly PropertyInfo _cypherVariableProperty;

        public EntityReturnColumns(PropertyInfo cypherVariableProperty)
        {
            _cypherVariableProperty = cypherVariableProperty;
        }

        public bool RequiresTypeProperty {get { return _cypherVariableProperty.PropertyType == typeof (Relationship); }}

        public string PropertiesPropertyName { get { return _cypherVariableProperty.Name; } }
        public string IdPropertyName { get { return _cypherVariableProperty.Name + "__Id"; } }
        public string TypePropertyName { get { return _cypherVariableProperty.Name + "__Type"; } }
    }
}
