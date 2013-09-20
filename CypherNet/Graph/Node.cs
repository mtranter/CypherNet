using System.Linq;

namespace CypherNet.Graph
{
    using System;
    using System.Collections.Generic;
    using Serialization;

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
        }

        public IList<string> Labels { get; private set; }
    }

}