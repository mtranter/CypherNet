namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    #endregion

    internal class CypherCreateRelationshipClauseBuilder
    {
        internal static string BuildCreateClause(Expression exp)
        {
            var lambda = exp as LambdaExpression;
            if (lambda == null)
            {
                throw new InvalidCypherMatchExpressionException();
            }

            return VisitExpression(ExpressionEvaluator.PartialEval(lambda.Body), "");
        }

        private static string VisitExpression(Expression expression, string currentClause)
        {
            if (expression is MethodCallExpression)
            {
                return VisitMethod((MethodCallExpression) expression, currentClause);
            }
            if (expression.Type.IsGenericType && expression.Type.GetGenericTypeDefinition() == typeof(ICreateRelationshipQueryContext<>))
            {
                return "";
            }

            throw new InvalidCypherMatchExpressionException();
        }

        private static string VisitMethod(MethodCallExpression expression, string currentClause)
        {
            currentClause = VisitExpression(expression.Object, currentClause);
            var argVals = MethodExpressionArgumentEvaluator.EvaluateArguments(expression);
            var matchFormat = expression.Method.GetCustomAttribute<ParseToCypherAttribute>().Format;
            return currentClause + String.Format(matchFormat, argVals);
        }
    }


    public class InvalidCypherCreateRelationshipExpressionException : Exception
    {
    }
}