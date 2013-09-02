
namespace CypherNet
{
    using System;
    using Graph;
    using Queries;

    public interface ICypherEndpoint
    {
        ICypherQueryStart<TVariables> BeginQuery<TVariables>();
        ICypherQueryStart<TVariables> BeginQuery<TVariables>(System.Linq.Expressions.Expression<Func<ICypherPrototype,TVariables>> variablePrototype);
        ICypherQueryReturnOnly<Node> CreateNode(object properties);
        ICypherQueryReturnOnly<Node> CreateNode(object properties, string label);
    }
}
