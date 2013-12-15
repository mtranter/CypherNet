namespace CypherNet.Serialization
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Graph;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using Queries;

    #endregion

    internal class CypherResultSetConverterFactoryJsonConverter : JsonConverter
    {
        private readonly IEntityCache _entityCache;
        internal CypherResultSetConverterFactoryJsonConverter(IEntityCache entityCache)
        {
            _entityCache = entityCache;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            var converter = GetConverter(objectType);
            return converter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof (CypherResultSet<>);
        }

        private JsonConverter GetConverter(Type type)
        {
            var baseType = type.GetGenericArguments()[0];
            var converterType = typeof (TransactionalResponseConverter<>).MakeGenericType(baseType);
            return (JsonConverter) Activator.CreateInstance(converterType, _entityCache);
        }

        private class TransactionalResponseConverter<TCypherResponse> :
            CustomCreationConverter<CypherResultSet<TCypherResponse>>
        {
            private readonly IEntityCache _cache;
            private const string ColumnsJsonProperty = "columns";
            private const string DataJsonProperty = "data";
            private readonly IDictionary<string, int> _propertyCache = new Dictionary<string, int>();
            private string[] _columns;

            public TransactionalResponseConverter(IEntityCache cache)
            {
                _cache = cache;
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override CypherResultSet<TCypherResponse> Create(Type objectType)
            {
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
                var isEmptyInArray = false;
                do
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        isEmptyInArray = true;
                    }
                    else if (reader.TokenType == JsonToken.EndArray && isEmptyInArray)
                    {
                        return null;
                    }
                    else if (reader.TokenType == JsonToken.PropertyName)
                    {
                        isEmptyInArray = false;
                        if (reader.Value.ToString().ToLower() == ColumnsJsonProperty.ToLower())
                        {
                            reader.Read();
                            _columns = serializer.Deserialize<string[]>(reader);
                            if (typeof (IGraphEntity).IsAssignableFrom(typeof (TCypherResponse)))
                            {
                                var propertyName = _columns.First(c => c.EndsWith("__Id")).Replace("__Id", "");
                                AddToPropertyCache(propertyName, typeof (TCypherResponse));
                            }
                            else
                            {
                                LoadPropertyCache();
                            }
                        }
                        else if (reader.Value.ToString().ToLower() == DataJsonProperty.ToLower())
                        {
                            reader.Read();
                            return new CypherResultSet<TCypherResponse>(Deserialize(reader, serializer).ToArray());
                        }
                    }
                } while (reader.Read());

                return null;
            }

            private void LoadPropertyCache()
            {
                foreach (var prop in typeof(TCypherResponse).GetProperties().Where(prop => _columns.Contains(prop.Name)))
                {
                    AddToPropertyCache(prop.Name, prop.PropertyType);
                }
            }

            void AddToPropertyCache(string propertyName, Type type)
            {
                var entityPropertyNames = new EntityReturnColumns(propertyName);
                _propertyCache.Add(propertyName, Array.IndexOf(_columns, propertyName));

                if (!typeof(IGraphEntity).IsAssignableFrom(type))
                {
                    return;
                }
                if (_columns.Contains(entityPropertyNames.IdPropertyName))
                {
                    _propertyCache.Add(entityPropertyNames.IdPropertyName, Array.IndexOf(_columns, entityPropertyNames.IdPropertyName));
                }
                if (type == typeof(Relationship))
                {
                    if (_columns.Contains(entityPropertyNames.TypePropertyName))
                    {
                        _propertyCache.Add(entityPropertyNames.TypePropertyName, Array.IndexOf(_columns, entityPropertyNames.TypePropertyName));
                    }
                }
                if (type != typeof(Node))
                {
                    return;
                }
                if (_columns.Contains(entityPropertyNames.LabelsPropertyName))
                {
                    _propertyCache.Add(entityPropertyNames.LabelsPropertyName, Array.IndexOf(_columns, entityPropertyNames.LabelsPropertyName));
                }
            }

            private IEnumerable<TCypherResponse> Deserialize(JsonReader reader, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    reader.Read();
                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        var record = serializer.Deserialize<JToken>(reader);
                        var items = new Dictionary<string, object>();
                        if (typeof(IGraphEntity).IsAssignableFrom(typeof(TCypherResponse)))
                        {
                            var propertyName = _columns.First(c => c.EndsWith("__Id")).Replace("__Id", "");
                            var graphEntity = LoadGraphEntity(propertyName, record, new Dictionary<string, object>(),
                                                              typeof(TCypherResponse));
                            reader.Read();
                            yield return (TCypherResponse)graphEntity;
                        }
                        else
                        {

                            foreach (var property in typeof(TCypherResponse).GetProperties())
                            {
                                var itemproperties = new Dictionary<string, object>();
                                var propertyType = property.PropertyType;
                                var propertyName = property.Name;
                                if (typeof(IGraphEntity).IsAssignableFrom(propertyType))
                                {
                                    var graphEntity = LoadGraphEntity(propertyName, record, itemproperties, propertyType);
                                    items.Add(propertyName, graphEntity);
                                }
                                else
                                {
                                    var restEntity = record["row"][_propertyCache[propertyName]];
                                    itemproperties.Add(propertyName, restEntity.ToObject(propertyType));
                                }
                            }

                            reader.Read();
                            yield return HydrateWithCtr<TCypherResponse>(items);
                        }
                    }
                }
            }

            private IGraphEntity LoadGraphEntity(string propertyName, JToken record, Dictionary<string, object> itemproperties,
                                                           Type propertyType)
            {
                IGraphEntity graphEntity = null;
                var entityPropertyNames = new EntityReturnColumns(propertyName);
                AssertNecesaryColumnForType(entityPropertyNames.IdPropertyName, typeof (IGraphEntity));

                var recordIsArray = record.GetType().IsAssignableFrom(typeof (JArray));
                var entityId = recordIsArray
                    ? record[_propertyCache[entityPropertyNames.IdPropertyName]].Value<long>() :
                    record["row"][_propertyCache[entityPropertyNames.IdPropertyName]].Value<long>();
                if (_cache.Contains(entityId, propertyType))
                {
                    graphEntity = _cache.GetEntity(entityId, propertyType);
                }
                else
                {
                    itemproperties.Add("id", entityId);

                    AssertNecesaryColumnForType(entityPropertyNames.PropertiesPropertyName, typeof (IGraphEntity));
                    var entityProperties =
                        recordIsArray
                            ? record[_propertyCache[entityPropertyNames.PropertiesPropertyName]]
                            .ToObject<Dictionary<string, object>>() :
                        record["row"][_propertyCache[entityPropertyNames.PropertiesPropertyName]].ToObject<Dictionary<string, object>>();
                    itemproperties.Add("properties", entityProperties);

                    if (typeof (Node).IsAssignableFrom(propertyType))
                    {
                        AssertNecesaryColumnForType(entityPropertyNames.LabelsPropertyName, typeof (Node));
                        var labels = entityProperties.Keys.ToArray();
                        itemproperties.Add("labels", labels);
                    }
                    else
                    {
                        AssertNecesaryColumnForType(entityPropertyNames.TypePropertyName,
                                                    typeof (Relationship));
                        var relType = recordIsArray ?
                            record[_propertyCache[entityPropertyNames.TypePropertyName]].ToObject<string>() :
                            record["row"][_propertyCache[entityPropertyNames.TypePropertyName]].ToObject<string>();
                        itemproperties.Add("type", relType);
                    }

                    graphEntity = (IGraphEntity) HydrateWithCtr(itemproperties, propertyType);
                    _cache.CacheEntity(graphEntity);
                }
                return graphEntity;
            }


            private TReturn HydrateWithCtr<TReturn>(IEnumerable<KeyValuePair<string, object>> values)
            {
                var types = typeof(TCypherResponse).GetProperties().Select(p => p.PropertyType).ToArray();
                var ctor =
                    typeof(TReturn).GetConstructor(
                                                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                                    null, types, null);
                return (TReturn)ctor.Invoke(values.Select(k => k.Value).ToArray());
            }

            private object HydrateWithCtr(IEnumerable<KeyValuePair<string, object>> values, Type returnType)
            {
                var types = values.Select(k => k.Value.GetType()).ToArray();
                var ctor = returnType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, types, null);
                return ctor.Invoke(values.Select(k => k.Value).ToArray());
            }

            private void AssertNecesaryColumnForType(string columnName, Type type)
            {
                if (!_propertyCache.Any(kvp => kvp.Key == columnName))
                {
                    throw new CypherColumnNotPresentException(columnName);
                }
            }
        }
    }
}