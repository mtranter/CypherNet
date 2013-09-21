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

    [CypherGraphEntityName("rels")]
    public interface IDefineCypherFromNode
    {
        [ParseToCypher("()")]
        IDefineCypherRelationship From();

        [ParseToCypher("({0})")]
        IDefineCypherRelationship From(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference);

        [ParseToCypher("(:{0})")]
        IDefineCypherRelationship From(string label);

        [ParseToCypher("({0}:{1})")]
        IDefineCypherRelationship From(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference, string label);
    }

    [CypherGraphEntityName("rels")]
    public interface IDefineCypherWithNode
    {
        [ParseToCypher("()")]
        IDefineCypherRelationship With();

        [ParseToCypher("({0})")]
        IDefineCypherRelationship With(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference);

        [ParseToCypher("(:{0})")]
        IDefineCypherRelationship With(string label);

        [ParseToCypher("({0}:{1})")]
        IDefineCypherRelationship With(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference, string label);
    }
}