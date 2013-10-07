
using System;
using System.Collections.Generic;
using System.Linq;
using CypherNet.Graph;

namespace CypherNet
{
    using System.Collections;

    internal interface IEntityCache
    {
        bool Contains<TEntity>(long entityId) where TEntity : class, IGraphEntity;
        bool Contains(long entityId, Type type);
        void CacheEntity(IGraphEntity entity);
        TEntity GetEntity<TEntity>(long entityId) where TEntity : class, IGraphEntity;
        IGraphEntity GetEntity(long entityId, Type entityType);
        void Clear();

        void Remove<TEntity>(long entityId) where TEntity : class, IGraphEntity;
        void Remove(long entityId, Type type);
    }

    internal class DictionaryEntityCache : IEntityCache
    {
        private readonly IDictionary<long, IDictionary<Type, IGraphEntity>> _inner = new Dictionary<long, IDictionary<Type, IGraphEntity>>();


        #region IEntityCache Members

        public bool Contains<TEntity>(long entityId) where TEntity : class, IGraphEntity
        {
            return Contains(entityId, typeof (TEntity));
        }

        public bool Contains(long entityId, Type type)
        {
            return _inner.ContainsKey(entityId) && _inner[entityId].ContainsKey(type);
        }

        public void CacheEntity(IGraphEntity entity)
        {
            var entType = entity.GetType();
            if (Contains(entity.Id, entType))
            {
                throw new Exception("Entry already exists in the cache");
            }
            var useExistingDictionary = _inner.ContainsKey(entity.Id);
            var dic = useExistingDictionary
                          ? _inner[entity.Id]
                          : new Dictionary<Type, IGraphEntity>();
            dic[entType] = entity;
            if (!useExistingDictionary)
            {
                _inner.Add(entity.Id, dic);
            }
        }

        public TEntity GetEntity<TEntity>(long entityId) where TEntity : class,IGraphEntity
        {
            var type = typeof (TEntity);
            if(!Contains(entityId, type))
            {
                return null;
            }

            return (TEntity)_inner[entityId][type];
        }

        public IGraphEntity GetEntity(long entityId, Type entityType)
        {
            if (!Contains(entityId, entityType))
            {
                return null;
            }

            return _inner[entityId][entityType];
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public void Remove<TEntity>(long entityId) where TEntity : class, IGraphEntity
        {
            Remove(entityId, typeof (TEntity));
        }

        public void Remove(long entityId, Type type)
        {
            if (!Contains(entityId, type))
            {
                return;
            }
            _inner[entityId].Remove(type);
        }

        #endregion

    }
}
