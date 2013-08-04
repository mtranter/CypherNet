namespace CypherNet.Graph
{
    using System.Collections.Generic;

    public class Node : GraphEntity<Node>
    {
        internal Node(long id, object properties)
            : base(id, properties)
        {
        }

        internal Node(long id, IDictionary<string, object> properties)
            : base(id, properties)
        {
        }
    }
}