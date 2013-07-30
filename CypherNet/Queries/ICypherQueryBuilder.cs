namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq;
    using System.Linq.Expressions;

    #endregion

    internal interface ICypherQueryBuilder
    {
        string BuildQueryString<TIn, TOut>(Expression<Action<TIn>> startClause,
                                           Expression
                                               <Func<TIn, IDefineCypherRelationship>>[]
                                               matchCaluses,
                                           Expression<Func<TIn, bool>> wherePredicate,
                                           Expression<Func<TIn, TOut>> returnClause);
    }

    internal class TransactionEndpointCypherQueryBuilder : ICypherQueryBuilder
    {
        public string BuildQueryString<TIn, TOut>(Expression<Action<TIn>> startClause,
                                                  Expression
                                                      <
                                                      Func<TIn, IDefineCypherRelationship>
                                                      >[] matchCaluses, Expression<Func<TIn, bool>> wherePredicate,
                                                  Expression<Func<TIn, TOut>> returnClause)
        {
            if (returnClause == null)
            {
                throw new ArgumentNullException("returnClause");
            }

            var start = startClause == null ? null : "START " + BuildStartClause(startClause);
            var match = matchCaluses == null ? null : "MATCH " + String.Join(", ", matchCaluses.Select(BuildMatchClause));
            var where = wherePredicate == null ? null : "WHERE " + BuildWhereClause(wherePredicate);
            var @return = "RETURN " + BuildReturnClause(returnClause);
            return String.Join(" ", new[] {start, match, where, @return}.Where(s => s != null));
        }

        internal string BuildStartClause(Expression exp)
        {
            return CypherStartClauseBuilder.BuildStartClause(exp);
        }

        internal string BuildMatchClause(Expression exp)
        {
            return CypherMatchClauseBuilder.BuildMatchClause(exp);
        }

        internal string BuildWhereClause(Expression exp)
        {
            return CypherWhereClauseBuilder.BuildWhereClause(exp);
        }

        internal string BuildReturnClause(Expression exp)
        {
            return CypherReturnsClauseBuilder.BuildReturnClause(exp);
        }
    }
}