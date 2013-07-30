namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Graph;

    #endregion

    internal class CypherReturnsClauseBuilder
    {
        internal static string BuildReturnClause(Expression expression)
        {
            var lambda = expression as LambdaExpression;
            if (lambda == null)
            {
                throw new InvalidCypherReturnsExpressionException();
            }
            expression = ExpressionEvaluator.PartialEval(expression);
            var body = lambda.Body;
            var newVal = body as NewExpression;

            return String.Join(", ", newVal.Arguments.Zip(newVal.Members, ParseExpressionToTerm));
        }

        private static string ParseExpressionToTerm(Expression node, MemberInfo memberInfo)
        {
            var constantExp = node as ConstantExpression;
            if (constantExp != null)
            {
                return constantExp.ToCypherString();
            }

            var member = node as MemberExpression;
            // new { prop = param.EntityRef }
            if (member != null)
            {
                return String.Format("{0} as {1}", member.Member.Name, memberInfo.Name);
            }

            var method = node as MethodCallExpression;
            // new { prop = param.GraphEntity.Prop<T>("prop") }
            if (method != null)
            {
                if (typeof(IGraphEntity).IsAssignableFrom(method.Method.DeclaringType))
                {
                    var methodMember = method.Object as MemberExpression;
                    if (methodMember != null)
                    {
                        var entityName = methodMember.Member.Name;
                        var args = MethodExpressionArgumentEvaluator.EvaluateArguments(method);
                        return String.Format("{0}.{1}? as {2}", entityName, args[0], memberInfo.Name);
                    }
                }

                var cypherFunctionAttribute = method.Method.GetCustomAttribute<ParseToCypherAttribute>();
                if (cypherFunctionAttribute != null)
                {
                    var @params = MethodExpressionArgumentEvaluator.EvaluateArguments(method);
                    return string.Format(cypherFunctionAttribute.Format, @params[0]) + " as " +
                           memberInfo.Name;
                }
            }

            throw new InvalidCypherReturnsExpressionException();
        }
    }

    public class InvalidCypherReturnsExpressionException : Exception
    {
    }
}