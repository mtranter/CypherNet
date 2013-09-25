using CypherNet.Graph;

namespace CypherNet.Queries
{
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
}