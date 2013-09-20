
namespace CypherNet
{
    using System;
    using Graph;
    using Queries;

    public interface ICypherSession
    {
        ICypherQueryStart<TVariables> BeginQuery<TVariables>();
        ICypherQueryStart<TVariables> BeginQuery<TVariables>(System.Linq.Expressions.Expression<Func<ICypherPrototype,TVariables>> variablePrototype);
        Node CreateNode(object properties);
        Node CreateNode(object properties, string label);
        Node GetNode(long id);
        void DeleteNode(long nodeId);
        void UpdateNode(long nodeId, object properties);
    }
}
