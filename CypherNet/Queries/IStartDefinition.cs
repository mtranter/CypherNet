namespace CypherNet.Queries
{
    #region

    using Graph;
    

    #endregion

    public static class Start
    {
        [ParseToCypherAttribute("{0}=node({1})")]
        public static void At([ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeRef, long id)
        {
        }

        [ParseToCypherAttribute("{0}=node:{1}({2})")]
        public static void At([ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeRef, string index,
                              string query)
        {
        }

        [ParseToCypherAttribute("{0}=relationship({1})")]
        public static void At(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationshipReference,
            long id)
        {
        }

        [ParseToCypherAttribute("{0}=relationship:{1}({2})")]
        public static void At(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationshipReference,
            string index, string query)
        {
        }

        [ParseToCypherAttribute("{0}=node(*)")]
        public static void Any([ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeRe)
        {
        }

        [ParseToCypherAttribute("{0}=relationship(*)")]
        public static void Any(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship nodeRe)
        {
        }
    }

    public interface IBeginRelationshipDefinition
    {
        [ParseToCypherAttribute("{0}=node({1})")]
        void At([ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeRef, long id);

        [ParseToCypherAttribute("{0}=node:{1}({2})")]
        void At([ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeRef, string index,
                              string query);

        [ParseToCypherAttribute("{0}=relationship({1})")]
        void At(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationshipReference,
            long id);

        [ParseToCypherAttribute("{0}=relationship:{1}({2})")]
        void At(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationshipReference,
            string index, string query);

        [ParseToCypherAttribute("{0}=node(*)")]
       void Any([ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeRe);

        [ParseToCypherAttribute("{0}=relationship(*)")]
        void Any(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship nodeRe);
    }
}