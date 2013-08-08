
namespace CypherNet
{
    using System;
    using Graph;
    using Queries;
    using Transaction;

    interface ITransactionalEndpoint : ICypherUnitOfWork
    {
        ICypherQueryStart<TVariables> BeginQuery<TVariables>();
        ICypherQueryStart<TVariables> BeginQuery<TVariables>(System.Linq.Expressions.Expression<Func<TVariables>> variablePrototype);
        ICypherQueryReturnOnly<Node> CreateNode(object properties);
        ICypherQueryReturnOnly<Node> CreateNode(object properties, string label);
    }
}
