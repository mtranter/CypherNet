namespace CypherNet.Queries
{
    #region

    using Graph;
    

    #endregion

    public interface IDefineCypherRelationship
    {
        [ParseToCypherAttribute("-[]->")]
        IDefineCypherToNode Outgoing();

        [ParseToCypherAttribute("-[:{0}]->")]
        IDefineCypherToNode Outgoing(string type);

        [ParseToCypherAttribute("-[:{0}*{1}..{2}]->")]
        IDefineCypherToNode Outgoing(string type, int minHops, int maxHops);

        [ParseToCypherAttribute("-[{0}]->")]
        IDefineCypherToNode Outgoing(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship hasActedIn);

        [ParseToCypherAttribute("-[{0}:{1}]->")]
        IDefineCypherToNode Outgoing(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship reference
            , string type);

        [ParseToCypherAttribute("<-[]-")]
        IDefineCypherFromNode Incoming();

        [ParseToCypherAttribute("<-[:{0}]-")]
        IDefineCypherFromNode Incoming(string type);

        [ParseToCypherAttribute("<-[:{0}*{1}..{2}]-")]
        IDefineCypherFromNode Incoming(string type, int minHops, int maxHops);

        [ParseToCypherAttribute("<-[{0}]-")]
        IDefineCypherFromNode Incoming(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship hasActedIn);

        [ParseToCypherAttribute("<-[{0}:{1}]-")]
        IDefineCypherFromNode Incoming(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Relationship reference, string type);

        [ParseToCypherAttribute("-[]-")]
        IDefineCypherWithNode Relates();

        [ParseToCypherAttribute("-[:{0}]-")]
        IDefineCypherWithNode Relates(string type);

        [ParseToCypherAttribute("-[:{0}*{1}..{2}]-")]
        IDefineCypherWithNode Relates(string type, int minHops, int maxHops);

        [ParseToCypherAttribute("-[{0}]-")]
        IDefineCypherWithNode Relates(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Relationship hasActedIn);

        [ParseToCypherAttribute("-[{0}:{1}]-")]
        IDefineCypherWithNode Relates(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] Relationship reference, string type);
    }
}