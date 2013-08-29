namespace CypherNet.Serialization
{
    public interface IWebSerializer
    {
        string Serialize(object objToSerialize);
        TItem Deserialize<TItem>(string json);
        dynamic ToDynmamic(string stream);
    }
}