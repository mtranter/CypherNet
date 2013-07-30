

namespace CypherNet.Serialization
{
    #region

    using System;
    using System.IO;
    using Dynamic4Neo.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    #endregion

    internal class DefaultJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializer _serializer;

        internal DefaultJsonSerializer()
        {
            _serializer = new JsonSerializer();
            _serializer.Converters.Insert(0, new CypherResultSetConverterFactoryJsonConverter());
        }

        #region IJsonSerializer Members

        public string Serialize(object objToSerialize)
        {
            var sw = new StringWriter();
            _serializer.Serialize(sw, objToSerialize);
            return sw.ToString();
        }

        public TItem Deserialize<TItem>(string json)
        {
            return _serializer.Deserialize<TItem>(new JsonTextReader(new StringReader(json)));
        }

        public dynamic ToDynmamic(string stream)
        {
            dynamic data = JObject.Parse(stream);
            return data.data;
        }

        public object Deserialize(string json, Type type)
        {
            return _serializer.Deserialize(new JsonTextReader(new StringReader(json)), type);
        }

        #endregion
    }
}