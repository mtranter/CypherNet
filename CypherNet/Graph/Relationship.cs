namespace CypherNet.Graph
{
    using System.Collections.Generic;

    public class Relationship : GraphEntity<Relationship>
    {
        internal Relationship(long id, object properties, string type)
            : base(id, properties)
        {
            Type = type;
        }

        internal Relationship(long id, IDictionary<string, object> properties)
            : base(id, properties)
        {
        }

        public string Type { get; set; }
    }
}