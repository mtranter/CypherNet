namespace CypherNet.Dynamic
{
    #region

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    #endregion

    public class CachedDictionaryPropertiesProvider
    {
        private static readonly IDictionary<Type, Action<object, IDictionary<string, object>>> _cache =
            new ConcurrentDictionary<Type, Action<object, IDictionary<string, object>>>();

        private static readonly Type DictionaryType = typeof (IDictionary<string, object>);
        private static readonly MethodInfo AddMethod = DictionaryType.GetMethod("Add");

        public static IDictionary<string, object> LoadProperties(object properties)
        {
            if (properties == null)
            {
                return null;
            }

            var type = properties.GetType();
            var action = _cache.ContainsKey(type) ? _cache[type] : (_cache[type] = BuildAction(properties));
            var retval = new Dictionary<string, object>();
            action(properties, retval);
            return retval;
        }

        private static Action<object, IDictionary<string, object>> BuildAction(object source)
        {
            if (source is IDictionary<string, object>)
            {
                return (s, d) => CloneDictionary(s as IDictionary<string, object>, d);
            }

            var sourceType = source.GetType();
            var properties = sourceType.GetProperties();
            var dictParam = Expression.Parameter(typeof (IDictionary<string, object>));
            var objParam = Expression.Parameter(typeof (object));
            var castVariable = Expression.Variable(sourceType);
            var castSource = Expression.Assign(castVariable, Expression.Convert(objParam, sourceType));
            var operations = new List<Expression> {castSource};

            foreach (var prop in properties)
            {
                var getter = Expression.Property(castVariable, prop);
                Expression value = getter;
                if (prop.PropertyType.IsValueType)
                {
                    value = Expression.Convert(getter, typeof (object));
                }
                var add = Expression.Call(dictParam, AddMethod, Expression.Constant(prop.Name), value);
                operations.Add(add);
            }

            var block = Expression.Block(new[] {castVariable}, operations.ToArray());
            var lambda = Expression.Lambda<Action<object, IDictionary<string, object>>>(block, objParam, dictParam);
            return lambda.Compile();
        }

        private static void CloneDictionary(IDictionary<string, object> source, IDictionary<string, object> target)
        {
            foreach (var kvp in source)
            {
                target[kvp.Key] = source[kvp.Key];
            }
        }
    }
}