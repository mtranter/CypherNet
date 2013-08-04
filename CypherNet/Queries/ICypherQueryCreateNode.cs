namespace CypherNet.Queries
{
    using Graph;

    public interface ICypherQueryCreateNode
    {
        ICypherQueryReturnOnly<Node> Create(object properties);
    }
}