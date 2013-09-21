namespace CypherNet.Queries
{
    #region

    using Graph;

    #endregion

    public static class Pattern
    {
        [ParseToCypher("()")]
        public static IDefineCypherRelationship Start()
        {
            throw new ExpressionTreeOnlyUsageException();
        }

        [ParseToCypher("({0})")]
        public static IDefineCypherRelationship Start(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference)
        {
            throw new ExpressionTreeOnlyUsageException();
        }

        [ParseToCypher("(:{0})")]
        public static IDefineCypherRelationship Start(string label)
        {
            throw new ExpressionTreeOnlyUsageException();
        }

        [ParseToCypher("({0}:{1})")]
        public static IDefineCypherRelationship Start(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference, string label)
        {
            throw new ExpressionTreeOnlyUsageException();
        }
    }
}