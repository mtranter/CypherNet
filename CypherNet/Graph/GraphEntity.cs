namespace CypherNet.Graph
{
    #region

    using System.Collections.Generic;
    using Dynamic;

    #endregion

    public abstract class GraphEntity<TEntity> : DynamicEntity<TEntity>, IGraphEntity
        where TEntity : DynamicEntity<TEntity>
    {
        internal GraphEntity(long id, object properties)
            : base(properties)
        {
            Id = id;
        }

        internal GraphEntity(long id, IDictionary<string, object> properties)
            : base(properties)
        {
            Id = id;
        }

        public long Id { get; internal set; }
    }
}