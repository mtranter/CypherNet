
using StaticReflection;

namespace CypherNet.Dynamic
{
    #region

    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    #endregion

    public abstract class DynamicEntity<TEntity> : IDynamicMetaObjectProvider, IDynamicMetaData
        where TEntity : DynamicEntity<TEntity>
    {

        // ReSharper disable StaticFieldInGenericType

        private static readonly MethodInfo SetMethodInfo =
            ReflectOn<DynamicEntity<TEntity>>.Member(m => m.SetDictionaryEntry("", null)).MemberInfo as MethodInfo;

        private static readonly MethodInfo GetMethodInfo =
            ReflectOn<DynamicEntity<TEntity>>.Member(m => m.GetDictionaryEntry("")).MemberInfo as MethodInfo;

        private static readonly IEnumerable<string> ExistingSetProperties =
            typeof (TEntity).GetProperties().Where(p => p.GetSetMethod() != null).Select(p => p.Name);

        private static readonly IEnumerable<string> ExistingGetProperties =
            typeof (TEntity).GetProperties().Where(p => p.GetGetMethod() != null).Select(p => p.Name);

        // ReSharper restore StaticFieldInGenericType

        private readonly IDictionary<string, object> _storage;
        
        protected internal DynamicEntity()
        {
            _storage = new Dictionary<string, object>();
        }

        protected internal DynamicEntity(IDictionary<string, object> properties)
        {
            _storage = properties;
        }

        protected internal DynamicEntity(object properties)
        {
            _storage = CachedDictionaryPropertiesProvider.LoadProperties(properties);
        }

        #region IDynamicMetaObjectProvider Members

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new DynamicDictionaryMetaObject(parameter, this);
        }

        #endregion

        bool IDynamicMetaData.HasProperty(string propName)
        {
            return _storage.ContainsKey(propName);
        }

        IEnumerable<string> IDynamicMetaData.AllPropertyNames
        {
            get { return _storage.Keys.AsEnumerable(); }
        }

        IEnumerable<KeyValuePair<string, object>> IDynamicMetaData.GetAllValues()
        {
            return _storage.AsEnumerable();
        }

        public override string ToString()
        {
            var message = new StringWriter();
            foreach (var item in _storage)
            {
                message.WriteLine("{0}:\t{1}", item.Key, item.Value);
            }
            return message.ToString();
        }

        private object SetDictionaryEntry(string key, object value)
        {
            if (_storage.ContainsKey(key))
            {
                if (!_storage[key].Equals(value))
                {
                    _storage[key] = value;
                    OnPropertyChanged(key, value);
                }
            }
            else
            {
                _storage.Add(key, value);
                OnPropertyChanged(key, value);
            }
            return value;
        }

        private object GetDictionaryEntry(string key)
        {
            object result = null;
            if (_storage.ContainsKey(key))
            {
                result = _storage[key];
            }
            return result;
        }

        protected virtual void OnPropertyChanged(string propertyName, object value)
        {
        }

        private class DynamicDictionaryMetaObject : DynamicMetaObject
        {
            internal DynamicDictionaryMetaObject(Expression parameter, DynamicEntity<TEntity> value)
                : base(parameter, BindingRestrictions.Empty, value)
            {
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                if (ExistingSetProperties.Contains(binder.Name))
                {
                    return base.BindSetMember(binder, value);
                }

                var restrictions =
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType);

                var args = new Expression[2];
                args[0] = Expression.Constant(binder.Name);
                args[1] = Expression.Convert(value.Expression, typeof (object));

                var self = Expression.Convert(Expression, LimitType);

                var methodCall = Expression.Call(self,
                                                 SetMethodInfo,
                                                 args);

                var setDictionaryEntry = new DynamicMetaObject(
                    methodCall,
                    restrictions);

                return setDictionaryEntry;
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                if (ExistingGetProperties.Contains(binder.Name))
                {
                    return base.BindGetMember(binder);
                }

                var parameters = new Expression[]
                                     {
                                         Expression.Constant(binder.Name)
                                     };

                var getDictionaryEntry = new DynamicMetaObject(
                    Expression.Call(
                                    Expression.Convert(Expression, LimitType),
                                    GetMethodInfo,
                                    parameters),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
                return getDictionaryEntry;
            }
        }
    }
}