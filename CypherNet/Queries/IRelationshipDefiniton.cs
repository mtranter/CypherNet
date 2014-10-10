namespace CypherNet.Queries
{
    #region

    using Graph;

    #endregion

    public interface IDefineCypherRelationship
    {
        [ParseToCypher("-[]->")]
        IDefineCypherToNode Outgoing();

        [ParseToCypher("-[:{0}]->")]
        IDefineCypherToNode Outgoing(string type);

        [ParseToCypher("-[:{0}*{1}..]->")]
        IDefineCypherToNode Outgoing(string type, int minHops);

        [ParseToCypher("-[:{0}*{1}..{2}]->")]
        IDefineCypherToNode Outgoing(string type, int minHops, int maxHops);

        [ParseToCypher("-[{0}]->")]
        IDefineCypherToNode Outgoing(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship hasActedIn);

        [ParseToCypher("-[{0}:{1}]->")]
        IDefineCypherToNode Outgoing(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship reference
            , string type);

        [ParseToCypher("<-[]-")]
        IDefineCypherFromNode Incoming();

        [ParseToCypher("<-[:{0}]-")]
        IDefineCypherFromNode Incoming(string type);

        [ParseToCypher("<-[:{0}*{1}..]-")]
        IDefineCypherFromNode Incoming(string type, int minHops);

        [ParseToCypher("<-[:{0}*{1}..{2}]-")]
        IDefineCypherFromNode Incoming(string type, int minHops, int maxHops);

        [ParseToCypher("<-[{0}]-")]
        IDefineCypherFromNode Incoming(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship hasActedIn);

        [ParseToCypher("<-[{0}:{1}]-")]
        IDefineCypherFromNode Incoming(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship reference, string type);

        [ParseToCypher("-[]-")]
        IDefineCypherWithNode Relates();

        [ParseToCypher("-[:{0}]-")]
        IDefineCypherWithNode Relates(string type);

        [ParseToCypher("-[:{0}*{1}..]-")]
        IDefineCypherWithNode Relates(string type, int minHops);

        [ParseToCypher("-[:{0}*{1}..{2}]-")]
        IDefineCypherWithNode Relates(string type, int minHops, int maxHops);

        [ParseToCypher("-[{0}]-")]
        IDefineCypherWithNode Relates(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship hasActedIn);

        [ParseToCypher("-[{0}:{1}]-")]
        IDefineCypherWithNode Relates(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship reference, string type);
    }
}