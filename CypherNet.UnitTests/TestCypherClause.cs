namespace Dynamic4Neo.Tests
{
    #region

    using CypherNet.Graph;

    #endregion

    public class TestCypherClause
    {
        public Node actor { get; set; }
        public Node director { get; set; }
        public Node movie { get; set; }
        public Relationship directedBy { get; set; }
        public Relationship actedIn { get; set; }
    }
}