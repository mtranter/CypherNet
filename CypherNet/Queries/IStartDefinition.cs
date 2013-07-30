namespace CypherNet.Queries
{
    #region

    using Graph;
    

    #endregion

    public interface IStartDefinition
    {
        [ParseToCypherAttribute("{0}=node({1})")]
        void At([ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeRef, long id);

        [ParseToCypherAttribute("{0}=node:{1}({2})")]
        void At([ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeRef, string index,
                string query);

        [ParseToCypherAttribute("{0}=relationship({1})")]
        void At(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationshipReference,
            long id);

        [ParseToCypherAttribute("{0}=relationship:{1}({2})")]
        void At(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Relationship relationshipReference,
            string index, string query);

        [ParseToCypherAttribute("{0}=node(*)")]
        void AnyNode([ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeRe);

        [ParseToCypherAttribute("{0}=relationship(*)")]
        void AnyRelationship(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Relationship nodeRe);
    }
}