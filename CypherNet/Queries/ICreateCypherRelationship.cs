namespace CypherNet.Queries
{
    #region

    using System;
    using Graph;

    #endregion

    public class Create
    {
        [ParseToCypher("({0})-[:{1}]->({2})")]
        public static ICreateCypherRelationship Relationship(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node start,
            string type,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node end)
        {
            throw new NotImplementedException();
        }

        [ParseToCypher("({0})-[{1}:{2}]->({3})")]
        public static ICreateCypherRelationship Relationship(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node start,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationship,
            string type,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node end)
        {
            throw new NotImplementedException();
        }

        [ParseToCypher("({0})-[{1}:{2} {3}]->({4})")]
        public static ICreateCypherRelationship Relationship(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node start,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationship,
            string type,
            [ArgumentEvaluator(typeof (JsonArgumentEvaluator))] object properties,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node end)
        {
            throw new NotImplementedException();
        }

        [ParseToCypher("({0})-[:{1} {2}]->({3})")]
        public static ICreateCypherRelationship Relationship(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node start,
            string type,
            [ArgumentEvaluator(typeof (JsonArgumentEvaluator))] object properties,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node end)
        {
            throw new NotImplementedException();
        }
    }
}