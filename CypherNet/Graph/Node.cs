namespace CypherNet.Graph
{
    using System;
    using System.Collections.Generic;
    using Serialization;

    public class Node : GraphEntity<Node>
    {
        internal Node(long id, object properties)
            : base(id, properties)
        {
        }

        [DeserializeUsing]
        internal Node(long id, IDictionary<string, object> properties)
            : base(id, properties)
        {
        }
    }

}