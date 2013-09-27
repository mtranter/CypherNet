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
            private static readonly PropertyInfo[] Properties = typeof (TCypherResponse).GetProperties();
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
                            LoadPropertyCache(_columns);
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

            private void LoadPropertyCache(string[] columns)
            {
                foreach (var prop in Properties)
                {
                    if (columns.Contains(prop.Name))
                    {
                        _propertyCache.Add(prop.Name, Array.IndexOf(columns, prop.Name));
                        if (typeof (IGraphEntity).IsAssignableFrom(prop.PropertyType))
                        {
                            var idProp = prop.Name + "__Id";
                            if (columns.Contains(idProp))
                            {
                                _propertyCache.Add(idProp, Array.IndexOf(columns, idProp));
                            }
                            if (prop.PropertyType == typeof (Relationship))
                            {
                                var typeProp = prop.Name + "__Type";
                                if (columns.Contains(typeProp))
                                {
                                    _propertyCache.Add(typeProp, Array.IndexOf(columns, typeProp));
                                }
                            }
                            if (prop.PropertyType == typeof (Node))
                            {
                                var typeProp = prop.Name + "__Labels";
                                if (columns.Contains(typeProp))
                                {
                                    _propertyCache.Add(typeProp, Array.IndexOf(columns, typeProp));
                                }
                            }
                        }
                    }
                }
            }

            private IEnumerable<TCypherResponse> Deserialize(JsonReader reader, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    reader.Read();
                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        var record = serializer.Deserialize<JToken[]>(reader);
                        var items = new Dictionary<string, object>();
                        foreach (var property in Properties)
                        {
                            var itemproperties = new Dictionary<string, object>();

                            if (typeof (IGraphEntity).IsAssignableFrom(property.PropertyType))
                            {
                                IGraphEntity graphEntity = null;
                                var entityPropertyNames = new EntityReturnColumns(property.Name);

                                AssertNecesaryColumnForType(entityPropertyNames.IdPropertyName, typeof (IGraphEntity));
                                var nodeId = record[_propertyCache[entityPropertyNames.IdPropertyName]].ToObject<long>();
                                if (_cache.Contains(nodeId))
                                {
                                    graphEntity = _cache.GetEntity(nodeId);
                                }
                                else
                                {

                                    itemproperties.Add("id", nodeId);

                                    AssertNecesaryColumnForType(entityPropertyNames.PropertiesPropertyName, typeof(IGraphEntity));
                                    var entityProperties = record[_propertyCache[entityPropertyNames.PropertiesPropertyName]].ToObject<Dictionary<string, object>>();
                                    itemproperties.Add("properties", entityProperties);

                                    if (entityPropertyNames.RequiresLabelsProperty)
                                    {
                                        AssertNecesaryColumnForType(entityPropertyNames.LabelsPropertyName, typeof(Node));
                                        var labels = record[_propertyCache[entityPropertyNames.LabelsPropertyName]].ToObject<string[]>();
                                        itemproperties.Add("labels", labels);
                                    }

                                    if (entityPropertyNames.RequiresTypeProperty)
                                    {
                                        AssertNecesaryColumnForType(entityPropertyNames.TypePropertyName,
                                                                    typeof(Relationship));
                                        var relType = record[_propertyCache[entityPropertyNames.TypePropertyName]].ToObject<string>();
                                        itemproperties.Add("type", relType);
                                    }

                                    graphEntity = (IGraphEntity)HydrateWithCtr(itemproperties, property.PropertyType);
                                    _cache.CacheEntity(graphEntity);
                                }
                                items.Add(property.Name, graphEntity);
                            }
                            else
                            {
                                var restEntity = record[_propertyCache[property.Name]];
                                itemproperties.Add(property.Name, restEntity.ToObject(property.PropertyType));
                            }
                        }
                        reader.Read();
                        yield return HydrateWithCtr<TCypherResponse>(items);
                    }
                }
            }


            private TReturn HydrateWithCtr<TReturn>(IEnumerable<KeyValuePair<string, object>> values)
            {
                var types = Properties.Select(p => p.PropertyType).ToArray();
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