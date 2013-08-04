
namespace CypherNet
{
    using System;
    using Graph;
    using Queries;
    using Transaction;

    interface ITransactionalEndpoint
    {
        ICypherQueryStart<TVariables> QueryUsing<TVariables>(System.Linq.Expressions.Expression<Func<TVariables>> func);
        ICypherQueryReturnOnly<Node> CreateNode(object properties);
        ICypherQueryReturnOnly<Node> CreateNode(object properties, string label);
    }
}
