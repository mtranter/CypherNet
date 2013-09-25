namespace CypherNet.Queries
{
    #region

    using Graph;

    #endregion

    [CypherGraphEntityName("rels")]
    public interface IDefineCypherToNode
    {
        [ParseToCypher("()")]
        IDefineCypherRelationship To();

        [ParseToCypher("({0})")]
        IDefineCypherRelationship To(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference);

        [ParseToCypher("(:{0})")]
        IDefineCypherRelationship To(string label);

        [ParseToCypher("({0}:{1})")]
        IDefineCypherRelationship To(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference, string label);
    }
}