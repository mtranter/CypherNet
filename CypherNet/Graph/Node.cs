﻿namespace CypherNet.Graph
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using Serialization;

    #endregion

    public class Node : GraphEntity<Node>
    {
        internal Node(long id, object properties, string[] labels)
            : base(id, properties)
        {
            Labels = labels.ToList();
        }

        [DeserializeUsing]
        internal Node(long id, IDictionary<string, object> properties, string[] labels)
            : base(id, properties)
        {
            Labels = labels.ToList();
            Properties = properties;
        }

        public IList<string> Labels { get; private set; }

        public IDictionary<string, object> Properties { get; private set; }  
    }
}