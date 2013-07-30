#region



#endregion

namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    #endregion

    internal class CypherStartClauseBuilder
    {
        private static readonly IEnumerable<MethodInfo> AllowedMethods = typeof (Start).GetMethods(BindingFlags.Static | BindingFlags.Public);

        internal static string BuildStartClause(Expression exp)
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
            if (!AllowedMethods.Contains(declareAssignMethod))
            {
                throw new InvalidCypherStartExpressionException();
            }

            var findMethodFormat =
                declareAssignMethod.GetCustomAttribute<ParseToCypherAttribute>().Format;
            var @params = MethodExpressionArgumentEvaluator.EvaluateArguments(body);
            return string.Format(findMethodFormat, @params);
        }
    }

    public class InvalidCypherStartExpressionException : Exception
    {
    }
}