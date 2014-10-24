namespace CypherNet.Serialization
{
    #region

    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    #endregion

    internal class DefaultJsonSerializer : IWebSerializer
    {
        private readonly JsonSerializer _serializer;

        internal DefaultJsonSerializer(IEntityCache cache)
        {
            _serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
            _serializer.Converters.Insert(0, new CypherResultSetConverterFactoryJsonConverter(cache));
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

        public object Deserialize(string json, Type type)
        {
            return _serializer.Deserialize(new JsonTextReader(new StringReader(json)), type);
        }

        #endregion
    }
}