
namespace CypherNet.Graph
{
    using System.Collections.Generic;
    using Dynamic;

    public interface IGraphEntity
    {
    }

    public abstract class GraphEntity<TEntity> : DynamicEntity<TEntity>, IGraphEntity
        where TEntity : DynamicEntity<TEntity>
    {
        internal GraphEntity(long id, object properties)
            : base(properties)
        {
            Id = id;
        }

        internal GraphEntity(long id, IDictionary<string,object> properties)
            : base(properties)
        {
            Id = id;
        }

        public long Id { get; internal set; }
    }

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
