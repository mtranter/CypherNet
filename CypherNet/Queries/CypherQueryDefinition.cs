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
}