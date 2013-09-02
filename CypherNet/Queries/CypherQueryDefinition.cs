namespace CypherNet.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;


    internal class CypherQueryDefinition<TIn, TOut>
    {
        private readonly List<Expression<Func<TIn, IDefineCypherRelationship>>> _matchClauses =
            new List<Expression<Func<TIn, IDefineCypherRelationship>>>();

        private readonly List<Expression<Func<TIn, dynamic>>> _orderByClauses =
            new List<Expression<Func<TIn, dynamic>>>();

        private readonly List<Expression<Action<TIn>>> _setterClauses =
            new List<Expression<Action<TIn>>>();

        internal Expression<Action<TIn>> StartClause { get; set; }

        internal Expression<Func<TIn, bool>> WherePredicate { get; set; }

        internal Expression<Func<TIn, TOut>> ReturnClause { get; set; }

        internal Expression<Func<TIn, ICreateCypherRelationship>> CreateRelationpClause { get; set; }

        internal IEnumerable<Expression<Func<TIn, dynamic>>> OrderByClauses { get { return _orderByClauses.AsEnumerable(); } }

        internal IEnumerable<Expression<Action<TIn>>> SetterClauses { get { return _setterClauses.AsEnumerable(); } }

        internal IEnumerable<Expression<Func<TIn, IDefineCypherRelationship>>> MatchClauses { get { return _matchClauses.AsEnumerable(); } }

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

        internal string BuildStatement()
        {
            if (ReturnClause == null)
            {
                throw new ArgumentNullException("ReturnClause");
            }

            var start = StartClause == null ? null : "START " + CypherStartClauseBuilder.BuildStartClause(StartClause);
            var match = MatchClauses.Any() ? "MATCH " + String.Join(", ", MatchClauses.Select(CypherMatchClauseBuilder.BuildMatchClause)) : null;
            var createRel = CreateRelationpClause == null ? null : "CREATE " + CypherCreateRelationshipClauseBuilder.BuildCreateClause(CreateRelationpClause);
            var where = WherePredicate == null ? null : "WHERE " + CypherWhereClauseBuilder.BuildWhereClause(WherePredicate);
            var setClause = SetterClauses.Any() ? "SET " + String.Join(" SET ", SetterClauses.Select(CypherSetClauseBuilder.BuildSetClause)) : null;
            var orderBy = OrderByClauses.Any() ? "ORDER BY " + String.Join(", ", OrderByClauses.Select(CypherOrderByClauseBuilder.BuildOrderByClause)) : null;

            var @return = "RETURN " + CypherReturnsClauseBuilder.BuildReturnClause(ReturnClause);
            var skip = Skip == null ? null : String.Format("SKIP {0}", Skip);
            var limit = Limit == null ? null : String.Format("LIMIT {0}", Limit);
            return String.Join(" ", new[] { start, createRel, match, where, setClause, @return, orderBy, skip, limit }.Where(s => s != null));
        }
    }
}