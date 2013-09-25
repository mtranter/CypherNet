using CypherNet.Graph;

namespace CypherNet.Queries
{
    public interface IStartQueryContext<out TVariables> : IQueryContext<TVariables>
    {

        [ParseToCypher("{0}=node({1})")]
        IStartQueryContext<TVariables> StartAtId(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeRef, long id);

        [ParseToCypher("{0}=node:{1}({2})")]
        IStartQueryContext<TVariables> StartAtIndex(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeRef, string index,
            string query);

        [ParseToCypher("{0}=relationship({1})")]
        IStartQueryContext<TVariables> StartAtId(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationshipReference,
            long id);

        [ParseToCypher("{0}=relationship:{1}({2})")]
        IStartQueryContext<TVariables> StartAtIndex(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationshipReference,
            string index, string query);

        [ParseToCypher("{0}=node(*)")]
        IStartQueryContext<TVariables> StartAtAny(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node nodeRe);

        [ParseToCypher("{0}=relationship(*)")]
        IStartQueryContext<TVariables> StartAtAny(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship nodeRe);
    }
}