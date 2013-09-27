namespace CypherNet.Queries
{
    #region

    using System;
    using Graph;

    #endregion

  public interface ICreateRelationshipQueryContext<out TVariables> : IQueryContext<TVariables>
    {
        [ParseToCypher("({0})-[:{1}]->({2})")]
      ICreateCypherRelationship CreateRel(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node start,
            string type,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node end);

        [ParseToCypher("({0})-[{1}:{2}]->({3})")]
        ICreateCypherRelationship CreateRel(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node start,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationship,
            string type,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node end);

        [ParseToCypher("({0})-[{1}:{2} {3}]->({4})")]
        ICreateCypherRelationship CreateRel(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node start,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Relationship relationship,
            string type,
            [ArgumentEvaluator(typeof (JsonArgumentEvaluator))] object properties,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node end);

        [ParseToCypher("({0})-[:{1} {2}]->({3})")]
        ICreateCypherRelationship CreateRel(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node start,
            string type,
            [ArgumentEvaluator(typeof (JsonArgumentEvaluator))] object properties,
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] Node end);
    }
}