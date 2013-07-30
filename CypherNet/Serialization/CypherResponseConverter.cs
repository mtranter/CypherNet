
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

    internal class CypherResponseConverterFactoryJsonConverter : JsonConverter
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
            return typeof (ICypherResponse).IsAssignableFrom(objectType);
        }

        private JsonConverter GetConverter(Type type)
        {
            var baseType = type.GetGenericArguments()[0];
            var converterType = typeof (TransactionalResponseConverter<>).MakeGenericType(baseType);
            return (JsonConverter) Activator.CreateInstance(converterType);
        }

        private class TransactionalResponseConverter<TCypherResponse> :
            CustomCreationConverter<CypherResponse<TCypherResponse>>
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

            public override CypherResponse<TCypherResponse> Create(Type objectType)
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
                            return new CypherResponse<TCypherResponse>(Deserialize(reader, serializer));
                        }
                    }
                }

                return null;
            }

            private void LoadPropertyCache(string[] columns)
            {
                var entityProperties =
                    Properties.Where(p =>
                                     typeof (Node).IsAssignableFrom(p.PropertyType) ||
                                     typeof (Relationship).IsAssignableFrom(p.PropertyType)).ToArray();

                var expectedProperties =
                    Properties.Select(s => s.Name);

                foreach (var prop in expectedProperties)
                {
                    if (columns.Contains(prop))
                    {
                        _propertyCache.Add(prop, Array.IndexOf(columns, prop));
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
                        var properties = new Dictionary<string, object>();
                        foreach (var property in Properties)
                        {
                            var restEntity = record[_propertyCache[property.Name]];
                            var container = restEntity as JContainer;
                            if(typeof(IGraphEntity).IsAssignableFrom(property.PropertyType))
                            {
                                var propertyProperty = property.Name;
                                AssertNecesaryColumnForType(propertyProperty, typeof(IGraphEntity));
                                var idProperty = property.Name + "__Id";
                                AssertNecesaryColumnForType(idProperty, typeof(IGraphEntity));
                                var entityProperties = container[propertyProperty].ToObject<Dictionary<string, object>>();
                                var nodeId = container[idProperty].ToObject<long>();
                                properties.Add("id", nodeId);
                                properties.Add("properties", entityProperties);

                                if (typeof (Relationship).IsAssignableFrom(property.PropertyType))
                                {
                                    var relTypeProperty = property.Name + "__Type";
                                    AssertNecesaryColumnForType(relTypeProperty, typeof(Relationship));
                                    var relType = container[relTypeProperty].ToObject<long>();
                                    properties.Add("type", relType);
                                }
                            }
                        else
                            {
                                properties.Add(property.Name, restEntity.ToObject(property.PropertyType));
                            }
                        }
                        reader.Read();
                        yield return HydrateWithCtr(properties);
                    }
                }
            }

            private TCypherResponse HydrateWithCtr(IEnumerable<KeyValuePair<string, object>> values)
            {
                var ctor = typeof (TCypherResponse).GetConstructor(Properties.Select(p => p.PropertyType).ToArray());
                return (TCypherResponse) ctor.Invoke(values.Select(k => k.Value).ToArray());
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
    
