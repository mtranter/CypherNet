namespace CypherNet.Configuration
{
    public interface ILabelStrategy
    {
        string GenerateLabel(object @object);
    }
}