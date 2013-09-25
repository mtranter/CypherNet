namespace CypherNet.Queries
{
    #region

    using Graph;

    #endregion


    public interface IMatchQueryContext<out TVaraibles> : IQueryContext<TVaraibles>
    {
        [ParseToCypher("()")]
        IDefineCypherRelationship Node();

        [ParseToCypher("({0})")]
        IDefineCypherRelationship Node(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference);

        [ParseToCypher("(:{0})")]
        IDefineCypherRelationship NodeLabelled(string label);

        [ParseToCypher("({0}:{1})")]
        IDefineCypherRelationship NodeLabelled(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference, string label);
    }
}