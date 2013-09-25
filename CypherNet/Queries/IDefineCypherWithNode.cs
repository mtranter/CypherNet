using CypherNet.Graph;

namespace CypherNet.Queries
{
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