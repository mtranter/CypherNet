namespace CypherNet.Queries
{
    #region

    using Graph;

    #endregion

    public static class Pattern
    {
        [ParseToCypherAttribute("()")]
        public static IDefineCypherRelationship Start()
        {
            throw new ExpressionTreeOnlyUsageException();
        }

        [ParseToCypherAttribute("({0})")]
        public static IDefineCypherRelationship Start(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeReference)
        {
            throw new ExpressionTreeOnlyUsageException();
        }

        [ParseToCypherAttribute("(:{0})")]
        public static IDefineCypherRelationship Start(string label)
        {
            throw new ExpressionTreeOnlyUsageException();
        }

        [ParseToCypherAttribute("({0}:{1})")]
        public static IDefineCypherRelationship Start(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeReference, string label)
        {
            throw new ExpressionTreeOnlyUsageException();
        }
    }
}