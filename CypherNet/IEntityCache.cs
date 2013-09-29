
using System;
using System.Collections.Generic;
using CypherNet.Graph;

namespace CypherNet
{
    internal interface IEntityCache
    {
        bool Contains(long entityId);
        void CacheEntity(IGraphEntity entity);
        IGraphEntity GetEntity(long entityId);
        void Clear();

        void Remove(long nodeId);
    }

    internal class DictionaryEntityCache : IEntityCache
    {
        private readonly IDictionary<long, IGraphEntity> _internalCache = new Dictionary<long, IGraphEntity>();

        public void CacheEntity(IGraphEntity entity)
        {
            if (_internalCache.ContainsKey(entity.Id))
            {
                throw new Exception("Cache already contains entity with Id: " + entity.Id);
            }

            _internalCache.Add(entity.Id, entity);
        }

        public IGraphEntity GetEntity(long entityId) 
        {
            IGraphEntity entity;
            _internalCache.TryGetValue(entityId, out entity);
            return entity;
        }

        public void Clear()
        {
            _internalCache.Clear();
        }

        public bool Contains(long entityId)
        {
            return _internalCache.ContainsKey(entityId);
        }

        public void Remove(long nodeId)
        {
            _internalCache.Remove(nodeId);
        }
    }
}
