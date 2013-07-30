namespace Dynamic4Neo.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize(object objToSerialize);
        TItem Deserialize<TItem>(string json);
        dynamic ToDynmamic(string stream);
    }
}