
namespace CypherNet.Configuration
{
    public class TypeLabelStrategy : ILabelStrategy
    {
        public string GenerateLabel(object @object)
        {
            return @object.GetType().ToString();
        }
    }
}
