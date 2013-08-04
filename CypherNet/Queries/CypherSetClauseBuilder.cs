using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherNet.Queries
{
    using System.Linq.Expressions;
    using System.Reflection;

    public static class CypherSetClauseBuilder
    {
        internal static string BuildSetClause(Expression exp)
        {
            var lambda = exp as LambdaExpression;
            if (lambda == null)
            {
                throw new InvalidCypherStartExpressionException();
            }

            var body = lambda.Body as MethodCallExpression;
            if (body == null)
            {
                throw new InvalidCypherStartExpressionException();
            }

            var declareAssignMethod = body.Method;
            
            var setFormat =
                declareAssignMethod.GetCustomAttribute<ParseToCypherAttribute>().Format;
            var @params = MethodExpressionArgumentEvaluator.EvaluateArguments(body);
            return string.Format(setFormat, @params);
        }
    }
}
