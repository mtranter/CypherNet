namespace CypherNet.Queries
{
    #region

    using Graph;

    #endregion

    public interface IBeginRelationshipDefinition
    {
        [ParseToCypherAttribute("()")]
        IDefineCypherRelationship Start();

        [ParseToCypherAttribute("({0})")]
        IDefineCypherRelationship Start(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference);

        [ParseToCypherAttribute("(:{0})")]
        IDefineCypherRelationship Start(string label);

        [ParseToCypherAttribute("({0}:{1})")]
        IDefineCypherRelationship Start(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeReference, string label);
    }
}