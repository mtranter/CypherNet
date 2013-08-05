namespace CypherNet.Queries
{
    #region

    using Graph;
    

    #endregion

    [CypherGraphEntityName("rels")]
    public interface IDefineCypherToNode : IBeginRelationshipDefinition
    {
        [ParseToCypherAttribute("()")]
        IDefineCypherRelationship To();

        [ParseToCypherAttribute("({0})")]
        IDefineCypherRelationship To(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeReference);

        [ParseToCypherAttribute("(:{0})")]
        IDefineCypherRelationship To(string label);

        [ParseToCypherAttribute("({0}:{1})")]
        IDefineCypherRelationship To(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeReference, string label);
    }

    [CypherGraphEntityName("rels")]
    public interface IDefineCypherFromNode : IBeginRelationshipDefinition
    {
        [ParseToCypherAttribute("()")]
        IDefineCypherRelationship From();

        [ParseToCypherAttribute("({0})")]
        IDefineCypherRelationship From(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeReference);
        
        [ParseToCypherAttribute("(:{0})")]
        IDefineCypherRelationship From(string label);

        [ParseToCypherAttribute("({0}:{1})")]
        IDefineCypherRelationship From(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeReference, string label);
    }

    [CypherGraphEntityName("rels")]
    public interface IDefineCypherWithNode : IBeginRelationshipDefinition
    {
        [ParseToCypherAttribute("()")]
        IDefineCypherRelationship With();

        [ParseToCypherAttribute("({0})")]
        IDefineCypherRelationship With(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeReference);

        [ParseToCypherAttribute("(:{0})")]
        IDefineCypherRelationship With(string label);

        [ParseToCypherAttribute("({0}:{1})")]
        IDefineCypherRelationship With(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Node nodeReference, string label);
    }
}