namespace CypherNet.Queries
{
    public interface ICypherEntityReference
    {
        TProp Prop<TProp>(string propertyName);
    }
}