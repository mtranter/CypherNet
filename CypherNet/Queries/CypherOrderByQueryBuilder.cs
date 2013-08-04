using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherNet.Queries
{
    using System.Linq.Expressions;
    using System.Reflection;
    using Graph;

    internal static class CypherOrderByClauseBuilder
    {
        internal static string BuildOrderByClause(Expression expression)
        {
            var lambda = expression as LambdaExpression;
            if (lambda == null)
            {
                throw new InvalidCypherReturnsExpressionException();
            }

            var body = lambda.Body;

            return ParseExpressionToTerm(body);
        }

        private static string ParseExpressionToTerm(Expression node)
        {

            if (node.NodeType == ExpressionType.Convert)
            {
                return ParseExpressionToTerm(((UnaryExpression) node).Operand);
            }

            var method = node as MethodCallExpression;
            // new { prop = param.GraphEntity.Prop<T>("prop") }
            if (method != null)
            {

                var cypherFunctionAttribute = method.Method.GetCustomAttribute<ParseToCypherAttribute>();
                if (cypherFunctionAttribute != null)
                {
                    var @params = MethodExpressionArgumentEvaluator.EvaluateArguments(method);
                    return string.Format(cypherFunctionAttribute.Format, @params);
                }

            }

            throw new InvalidOrderByClauseException();
        }
    }

    internal class InvalidOrderByClauseException : Exception
    {
    }
}

