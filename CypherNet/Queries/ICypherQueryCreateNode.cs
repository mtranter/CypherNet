namespace CypherNet.Queries
{
    #region

    using Graph;

    #endregion

    public interface ICypherQueryCreateNode
    {
        ICypherQueryReturnOnly<Node> Create(object properties);
    }
}