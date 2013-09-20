
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
    }
}
