using StaticReflection.Extensions;

namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Graph;
    using Serialization;

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
            var body = lambda.Body;
            var newVal = body as NewExpression;
            if (newVal != null)
            {
                return String.Join(", ", newVal.Arguments.Zip(newVal.Members, ParseExpressionToTerm));
            }
            var prop = body as MemberExpression;
            if (prop != null)
            {
                var entityPropertyNames = new EntityReturnColumns(prop.Member.Name);

                return BuildStatement(prop.Member.Name, entityPropertyNames, typeof(Relationship).IsAssignableFrom(((PropertyInfo)prop.Member).PropertyType));
            }

            var memberInit = body as MemberInitExpression;
            if (memberInit != null)
            {
                var statement = string.Join(", ", memberInit.Bindings.Select(
                    b =>
                        BuildStatement(b.Member.Name, new EntityReturnColumns(b.Member.Name),
                            typeof (Relationship).IsAssignableFrom(((PropertyInfo) b.Member).PropertyType))));
                return statement;
            }

            throw new InvalidCypherReturnsExpressionException();
        }

        private static string ParseExpressionToTerm(Expression node, MemberInfo memberInfo)
        {
            node = ExpressionEvaluator.PartialEval(node);
            var constantExp = node as ConstantExpression;
            if (constantExp != null)
            {
                return constantExp.ToCypherString() + " as " + memberInfo.Name;
            }

            var member = node as MemberExpression;
            // new { prop = param.EntityRef }
            if (member != null)
            {
                var entityName = member.Member.Name;
                var entityPropertyNames = new EntityReturnColumns(memberInfo.Name);

                return BuildStatement(entityName, entityPropertyNames, typeof(Relationship).IsAssignableFrom(((PropertyInfo)member.Member).PropertyType));
            }

            var method = node as MethodCallExpression;
            // new { prop = param.GraphEntity.Prop<T>("prop") }
            if (method != null)
            {
                if (typeof (IGraphEntity).IsAssignableFrom(method.Method.DeclaringType))
                {
                    var methodMember = method.Object as MemberExpression;
                    if (methodMember != null)
                    {
                        var entityName = methodMember.Member.Name;
                        var args = MethodExpressionArgumentEvaluator.EvaluateArguments(method);
                        var retval = String.Format("{0}.{1}? as {2}", entityName, args[0], memberInfo.Name);
                        return retval;
                    }
                }

                var cypherFunctionAttribute = method.Method.GetCustomAttribute<ParseToCypherAttribute>();
                if (cypherFunctionAttribute != null)
                {
                    var @params = MethodExpressionArgumentEvaluator.EvaluateArguments(method);
                    return string.Format(cypherFunctionAttribute.Format, @params) + " as " +
                           memberInfo.Name;
                }
            }

            throw new InvalidCypherReturnsExpressionException();
        }

        private static string BuildStatement(string entityName, EntityReturnColumns entityPropertyNames, bool isRelationship)
        {
            var retval = String.Format("{0} as {1}, id({0}) as {2}", entityName,
                                       entityPropertyNames.PropertiesPropertyName,
                                       entityPropertyNames.IdPropertyName);
            if (isRelationship)
            {
                retval += String.Format(", type({0}) as {1}", entityName, entityPropertyNames.TypePropertyName);
            }
            else
            {
                retval += String.Format(", labels({0}) as {1}", entityName, entityPropertyNames.LabelsPropertyName);
            }
            return retval;
        }
    }

    public class InvalidCypherReturnsExpressionException : Exception
    {
    }
}