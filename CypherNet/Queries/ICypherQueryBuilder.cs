namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    #endregion

    internal interface ICypherQueryBuilder
    {
        string BuildQueryString<TIn, TOut>(CypherQueryDefinition<TIn, TOut> queryDefinition);
    }

    internal class CypherQueryDefinition<TIn, TOut>
    {
        private readonly List<Expression<Func<TIn, IDefineCypherRelationship>>> _matchClauses =
            new List<Expression<Func<TIn, IDefineCypherRelationship>>>();

        private readonly List<Expression<Func<TIn, dynamic>>> _orderByClauses =
            new List<Expression<Func<TIn, dynamic>>>();

        private readonly List<Expression<Action<TIn>>> _setterClauses =
            new List<Expression<Action<TIn>>>();

        internal Expression<Action<TIn>> StartClause { get; set; }
        internal IEnumerable<Expression<Func<TIn, IDefineCypherRelationship>>> MatchClauses {
            get { return _matchClauses.AsEnumerable(); }
        }
        internal Expression<Func<TIn, bool>> WherePredicate { get; set; }
        internal Expression<Func<TIn, TOut>> ReturnClause { get; set; }
        internal IEnumerable<Expression<Func<TIn, dynamic>>> OrderByClauses { get { return _orderByClauses.AsEnumerable(); }}

        internal IEnumerable<Expression<Action<TIn>>> SetterClauses { get { return _setterClauses.AsEnumerable(); } }

        internal int? Skip { get; set; }
        internal int? Limit { get; set; }

        internal void AddMatchClause(Expression<Func<TIn, IDefineCypherRelationship>> match)
        {
            _matchClauses.Add(match);
        }

        internal void AddOrderByClause(Expression<Func<TIn, dynamic>> match)
        {
            _orderByClauses.Add(match);
        }

        internal void AddSetClause(Expression<Action<TIn>> match)
        {
            _setterClauses.Add(match);
        }
    }

    internal class TransactionEndpointCypherQueryBuilder : ICypherQueryBuilder
    {
        public string BuildQueryString<TIn, TOut>(CypherQueryDefinition<TIn, TOut> queryDefinition)
        {
            if (queryDefinition.ReturnClause == null)
            {
                throw new ArgumentNullException("ReturnClause");
            }

            var start = queryDefinition.StartClause == null ? null : "START " + BuildStartClause(queryDefinition.StartClause);
            var match = queryDefinition.MatchClauses.Any() ? "MATCH " + String.Join(", ", queryDefinition.MatchClauses.Select(BuildMatchClause)) : null;
            var where = queryDefinition.WherePredicate == null ? null : "WHERE " + BuildWhereClause(queryDefinition.WherePredicate);
            var setClause = queryDefinition.SetterClauses.Any() ? "SET " + String.Join(" SET ",queryDefinition.SetterClauses.Select(BuildSetClause)) : null;
            var orderBy = queryDefinition.OrderByClauses.Any() ? "ORDER BY " + String.Join(", ", queryDefinition.OrderByClauses.Select(BuildOrderByClause)) : null;
            
            var @return = "RETURN " + BuildReturnClause(queryDefinition.ReturnClause);
            var skip = queryDefinition.Skip == null ? null : String.Format("SKIP {0}", queryDefinition.Skip);
            var limit = queryDefinition.Limit == null ? null : String.Format("LIMIT {0}", queryDefinition.Limit);
            return String.Join(" ", new[] { start, match, where, setClause, @return, orderBy, skip, limit }.Where(s => s != null));
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

        internal string BuildOrderByClause(Expression exp)
        {
            return CypherOrderByClauseBuilder.BuildOrderByClause(exp);
        }

        internal string BuildSetClause(Expression exp)
        {
            return CypherSetClauseBuilder.BuildSetClause(exp);
        }
    }
}