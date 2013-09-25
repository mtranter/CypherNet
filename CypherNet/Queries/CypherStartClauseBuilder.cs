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

            return BuildStartClause(body, null);
        }

        private static string BuildStartClause(MethodCallExpression body, string currentExpression)
        {
            var declareAssignMethod = body.Method;
            if (declareAssignMethod.DeclaringType.GetGenericTypeDefinition() != typeof(IStartQueryContext<>))
            {
                throw new InvalidCypherStartExpressionException();
            }

            if (body.Object is MethodCallExpression)
            {
                currentExpression = BuildStartClause((MethodCallExpression) body.Object, currentExpression);
            }

            var findMethodFormat =
                declareAssignMethod.GetCustomAttribute<ParseToCypherAttribute>().Format;
            var @params = MethodExpressionArgumentEvaluator.EvaluateArguments(body);
            var thisAssignment = string.Format(findMethodFormat, @params);
            return String.Join(", ", new[] {currentExpression, thisAssignment}.Where(s => !String.IsNullOrEmpty(s)));
        }
    }

    public class InvalidCypherStartExpressionException : Exception
    {
    }
}