namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Graph;
    using StaticReflection;

    #endregion

    internal class CypherWhereClauseBuilder
    {
        private static readonly MemberInfo NodeIdMember = ReflectOn<Node>.Member(n => n.Id).MemberInfo;
        private static readonly MemberInfo RelIdMember = ReflectOn<Relationship>.Member(n => n.Id).MemberInfo;

        internal static string BuildWhereClause(Expression exp)
        {
            var lambda = exp as LambdaExpression;
            if (lambda == null)
            {
                throw new InvalidCypherWhereExpressionException();
            }
            var visitor = new WhereClauseVisitor();
            return visitor.Translate(lambda.Body);
        }

        internal class WhereClauseVisitor : ExpressionVisitor
        {
            private StringBuilder _queryBuilder;

            internal string Translate(Expression expression)
            {
                expression = ExpressionEvaluator.PartialEval(expression);
                _queryBuilder = new StringBuilder();
                Visit(expression);
                return _queryBuilder.ToString();
            }


            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                var cypherFunctionAttribute = m.Method.GetCustomAttribute<ParseToCypherAttribute>();
                if (cypherFunctionAttribute != null)
                {
                    var @params = MethodExpressionArgumentEvaluator.EvaluateArguments(m);
                    _queryBuilder.Append(string.Format(cypherFunctionAttribute.Format, @params));
                    return m;
                }

                throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
            }

            protected override Expression VisitUnary(UnaryExpression u)
            {
                switch (u.NodeType)
                {
                    case ExpressionType.Not:
                        _queryBuilder.Append(" NOT(");
                        Visit(u.Operand);
                        _queryBuilder.Append(")");
                        break;
                    default:
                        throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported",
                                                                      u.NodeType));
                }
                return u;
            }

            protected override Expression VisitBinary(BinaryExpression b)
            {
                _queryBuilder.Append("(");
                Visit(b.Left);
                switch (b.NodeType)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        _queryBuilder.Append(" AND ");
                        break;
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        _queryBuilder.Append(" OR ");
                        break;
                    case ExpressionType.Equal:
                        _queryBuilder.Append(" = ");
                        break;
                    case ExpressionType.NotEqual:
                        _queryBuilder.Append(" <> ");
                        break;
                    case ExpressionType.LessThan:
                        _queryBuilder.Append(" < ");
                        break;
                    case ExpressionType.LessThanOrEqual:
                        _queryBuilder.Append(" <= ");
                        break;
                    case ExpressionType.GreaterThan:
                        _queryBuilder.Append(" > ");
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        _queryBuilder.Append(" >= ");
                        break;
                    default:
                        throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported",
                                                                      b.NodeType));
                }
                Visit(b.Right);
                _queryBuilder.Append(")");
                return b;
            }

            protected override Expression VisitConstant(ConstantExpression c)
            {
                _queryBuilder.Append(c.ToCypherString());
                return c;
            }


            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
                {
                    _queryBuilder.Append(node.Member.Name);
                    return node;
                }
                else if (new[] {NodeIdMember, RelIdMember}.Any(m => m == node.Member) &&
                         node.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    var parent = node.Expression as MemberExpression;
                    _queryBuilder.AppendFormat("id({0})", parent.Member.Name);
                    return node;
                }

                throw new NotSupportedException(string.Format("The member '{0}' is not supported", node.Member.Name));
            }
        }
    }


    public class InvalidCypherWhereExpressionException : Exception
    {
    }
}