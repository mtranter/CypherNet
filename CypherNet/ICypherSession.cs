namespace CypherNet
{
    #region

    using System;
    using System.Linq.Expressions;
    using Graph;
    using Queries;

    #endregion

    public interface ICypherSession
    {
        Node GetNode(long id);

        Node CreateNode(object properties);
        Node CreateNode(object properties, string label);

        Relationship CreateRelationship(Node node1, Node node2, string type, object relationshipProperties = null);
        void DeleteRelationship(Relationship relationship);
        void DeleteRelationship(long relationshipId);
        
        void Delete(Node node);
        void Delete(long nodeId);
        void Save(Node node);

        void Clear();

        ICypherQueryStart<TVariables> BeginQuery<TVariables>();
        ICypherQueryStart<TVariables> BeginQuery<TVariables>(Expression<Func<ICypherPrototype, TVariables>> variablePrototype);
    }
}