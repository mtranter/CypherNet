
namespace CypherNet.Serialization
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CypherNet.Graph;
    using CypherNet.Queries;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;

    #endregion

    internal class CypherResultSetConverterFactoryJsonConverter : JsonConverter
    {
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
            return (JsonConverter) Activator.CreateInstance(converterType);
        }

        private class TransactionalResponseConverter<TCypherResponse> :
            CustomCreationConverter<CypherResultSet<TCypherResponse>>
        {
            private const string ColumnsJsonProperty = "columns";
            private const string DataJsonProperty = "data";
            private static readonly PropertyInfo[] Properties = typeof (TCypherResponse).GetProperties();
            private readonly IDictionary<string, int> _propertyCache = new Dictionary<string, int>();
            private string[] _columns;

            public override bool CanWrite
            {
                get { return false; }
            }

            public override CypherResultSet<TCypherResponse> Create(Type objectType)
            {
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                            JsonSerializer serializer)
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
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
                }

                return null;
            }

            private void LoadPropertyCache(string[] columns)
            {

                foreach (var prop in Properties)
                {
                    if (columns.Contains(prop.Name))
                    {
                        _propertyCache.Add(prop.Name, Array.IndexOf(columns, prop.Name));
                        if (typeof(IGraphEntity).IsAssignableFrom(prop.PropertyType))
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
                            var restEntity = record[_propertyCache[property.Name]];
                            if(typeof(IGraphEntity).IsAssignableFrom(property.PropertyType))
                            {
                                var propertyProperty = property.Name;
                                AssertNecesaryColumnForType(propertyProperty, typeof(IGraphEntity));
                                var idProperty = property.Name + "__Id";
                                AssertNecesaryColumnForType(idProperty, typeof(IGraphEntity));
                                var entityProperties = record[_propertyCache[propertyProperty]].ToObject<Dictionary<string, object>>();
                                var nodeId =  record[_propertyCache[idProperty]].ToObject<long>();
                                itemproperties.Add("id", nodeId);
                                itemproperties.Add("properties", entityProperties);

                                if (typeof (Relationship).IsAssignableFrom(property.PropertyType))
                                {
                                    var relTypeProperty = property.Name + "__Type";
                                    AssertNecesaryColumnForType(relTypeProperty, typeof(Relationship));
                                    var relType = record[_propertyCache[relTypeProperty]].ToObject<string>();
                                    itemproperties.Add("type", relType);
                                }
                                var entity = HydrateWithCtr(itemproperties, property.PropertyType);
                                items.Add(property.Name, entity);
                            }
                        else
                            {
                                itemproperties.Add(property.Name, restEntity.ToObject(property.PropertyType));
                            }
                        }
                        reader.Read();
                        yield return HydrateWithCtr<TCypherResponse>(items);
                    }
                }
            }

            private object HydrateWithCtr(IEnumerable<KeyValuePair<string, object>> values, Type returnType)
            {
                var types = values.Select(k => k.Value.GetType()).ToArray();
                var ctor = returnType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, types, null);
                return  ctor.Invoke(values.Select(k => k.Value).ToArray());
            }


            private TReturn HydrateWithCtr<TReturn>(IEnumerable<KeyValuePair<string, object>> values)
            {
                var types = Properties.Select(p => p.PropertyType).ToArray();
                var ctor = typeof(TReturn).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, types, null);
                return (TReturn)ctor.Invoke(values.Select(k => k.Value).ToArray());
            }

            private void AssertNecesaryColumnForType(string columnName, Type type)
            {
                if(!_propertyCache.Any(kvp => kvp.Key == columnName))
                {
                    throw new CypherColumnNotPresentException(columnName);
                }
            }
        }
    }


}
    
