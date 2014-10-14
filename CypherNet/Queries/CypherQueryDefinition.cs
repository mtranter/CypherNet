namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    #endregion

    internal class CypherQueryDefinition<TIn, TOut>
    {
        private readonly List<Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>> _matchClauses =
            new List<Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>>();

        private readonly List<Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>> _optionalMatchClauses =
            new List<Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>>();

        private readonly List<Expression<Func<TIn, dynamic>>> _orderByClauses =
            new List<Expression<Func<TIn, dynamic>>>();

        private readonly List<Expression<Func<IUpdateQueryContext<TIn>, ISetResult>>> _setterClauses =
            new List<Expression<Func<IUpdateQueryContext<TIn>, ISetResult>>>();

        internal Expression<Action<IStartQueryContext<TIn>>> StartClause { get; set; }

        internal Expression<Func<IWhereQueryContext<TIn>, bool>> WherePredicate { get; set; }

        internal Expression<Func<IReturnQueryContext<TIn>, TOut>> ReturnClause { get; set; }

        internal Expression<Func<TIn, TOut>> DeleteClause { get; set; }

        internal Expression<Func<ICreateRelationshipQueryContext<TIn>, ICreateCypherRelationship>> CreateRelationpClause { get; set; }

        internal IEnumerable<Expression<Func<TIn, dynamic>>> OrderByClauses
        {
            get { return _orderByClauses.AsEnumerable(); }
        }

        internal IEnumerable<Expression<Func<IUpdateQueryContext<TIn>, ISetResult>>> SetterClauses
        {
            get { return _setterClauses.AsEnumerable(); }
        }

        internal IEnumerable<Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>> MatchClauses
        {
            get { return _matchClauses.AsEnumerable(); }
        }

        internal IEnumerable<Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>> OptionalMatchClauses
        {
            get { return _optionalMatchClauses.AsEnumerable(); }
        }

        internal int? Skip { get; set; }
        internal int? Limit { get; set; }

        internal void AddMatchClause(Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>> match)
        {
            _matchClauses.Add(match);
        }

        internal void AddOptionalMatchClause(Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>> match)
        {
            _optionalMatchClauses.Add(match);
        }

        internal void AddOrderByClause(Expression<Func<TIn, dynamic>> match)
        {
            _orderByClauses.Add(match);
        }

        internal void AddSetClause(Expression<Func<IUpdateQueryContext<TIn>, ISetResult>> match)
        {
            _setterClauses.Add(match);
        }

        internal CypherQueryDefinition<TIn,TOutNew> Copy<TOutNew>()
        {
            var query = new CypherQueryDefinition<TIn, TOutNew>();
            foreach (var match in _matchClauses)
            {
                query._matchClauses.Add(match);
            }
            foreach (var setter in _setterClauses)
            {
                query._setterClauses.Add(setter);
            }
            foreach (var orderBy in _orderByClauses)
            {
                query._orderByClauses.Add(orderBy);
            }
            query.StartClause = this.StartClause;
            query.WherePredicate = this.WherePredicate;
            query.CreateRelationpClause = this.CreateRelationpClause;
            query.Limit = this.Limit;
            query.Skip = this.Skip;
            return query;
        }

        internal string BuildStatement()
        {
            var start = StartClause == null ? null : "START " + CypherStartClauseBuilder.BuildStartClause(StartClause);
            var match = MatchClauses.Any()
                            ? "MATCH " +
                              String.Join(", ", MatchClauses.Select(CypherMatchClauseBuilder.BuildMatchClause))
                            : null;
            var optionalMatch = OptionalMatchClauses.Any()
                            ? "OPTIONAL MATCH " +
                              String.Join(", ", OptionalMatchClauses.Select(CypherMatchClauseBuilder.BuildMatchClause))
                            : null;
            var createRel = CreateRelationpClause == null
                                ? null
                                : "CREATE " +
                                  CypherCreateRelationshipClauseBuilder.BuildCreateClause(CreateRelationpClause);
            var where = WherePredicate == null
                            ? null
                            : "WHERE " + CypherWhereClauseBuilder.BuildWhereClause(WherePredicate);
            var setClause = SetterClauses.Any()
                                ? "SET " +
                                  String.Join(" SET ", SetterClauses.Select(CypherSetClauseBuilder.BuildSetClause))
                                : null;
            var orderBy = OrderByClauses.Any()
                              ? "ORDER BY " +
                                String.Join(", ", OrderByClauses.Select(CypherOrderByClauseBuilder.BuildOrderByClause))
                              : null;

            var @return = ReturnClause == null ? null : "RETURN " + CypherReturnsClauseBuilder.BuildReturnClause(ReturnClause);
            var @delete = DeleteClause == null ? null : "DELETE " + CypherDeleteClauseBuilder.BuildDeleteClause(DeleteClause);
            var skip = Skip == null ? null : String.Format("SKIP {0}", Skip);
            var limit = Limit == null ? null : String.Format("LIMIT {0}", Limit);
            return String.Join(" ",
                               new[] { start, createRel, match, optionalMatch, where, setClause, @return, @delete, orderBy, skip, limit }.Where(s => s != null));
        }
    }
}