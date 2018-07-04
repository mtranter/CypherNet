using Newtonsoft.Json;

namespace CypherNet.Serialization
{
    public interface IWebSerializer
    {
        JsonConverterCollection JsonConverters { get; }
        string Serialize(object objToSerialize);
        TItem Deserialize<TItem>(string json);
    }
}