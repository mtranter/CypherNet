namespace CypherNet.Queries
{
    #region

    using System;
    using Graph;

    #endregion

    public static class CypherFunctions
    {
        [ParseToCypher("type({0})")]
        public static string Type(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationshipReference)
        {
            throw new NotImplementedException();
        }

        [ParseToCypher("has({0}.{1})")]
        public static bool Has(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] IGraphEntity relationshipReference,
            string property)
        {
            throw new NotImplementedException();
        }

        [ParseToCypher("id({0})")]
        public static long Id(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] IGraphEntity relationshipReference)
        {
            throw new NotImplementedException();
        }
    }
}