namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Graph;

    #endregion

    internal static class CypherQuery
    {
        internal static readonly Type[] ValidGraphEntityTypes = new[] {typeof (IGraphEntity)};
    }

    internal class FluentCypherQueryBuilder<TIn> : ICypherQueryStart<TIn>, ICypherQueryMatch<TIn>,
                                                   ICypherQueryWhere<TIn>, ICypherQueryReturns<TIn>
    {
        private readonly ICypher _cypherEndpoint;
        private readonly string _cypherQuery = String.Empty;
        private readonly ICypherQueryBuilder _queryBuilder;
        private Expression<Func<TIn, IDefineCypherRelationship>>[] _matchClauses;
        private Expression<Action<TIn>> _startDef;
        private Expression<Func<TIn, bool>> _wherePredicate;

        internal FluentCypherQueryBuilder(ICypher cypherEndpoint, ICypherQueryBuilder queryBuilder)
        {
            _cypherEndpoint = cypherEndpoint;
            _queryBuilder = queryBuilder;
        }

        internal string CypherQuery
        {
            get { return _cypherQuery; }
        }

        public ICypherQueryMatch<TIn> Start(Expression<Action<TIn>> startDef)
        {
            _startDef = startDef;
            return this;
        }

        public ICypherQueryWhere<TIn> Match(
            params Expression<Func<TIn, IDefineCypherRelationship>>[] matchDefs)
        {
            _matchClauses = matchDefs;
            return this;
        }

        public ICypherQueryReturns<TIn> Where(Expression<Func<TIn, bool>> predicate)
        {
            _wherePredicate = predicate;
            return this;
        }

        public ICypherQueryReturns<TIn> Where()
        {
            return this;
        }

        public ICypherExecute<TOut> Return<TOut>(Expression<Func<TIn, TOut>> func)
        {
            var query = _queryBuilder.BuildQueryString(_startDef, _matchClauses, _wherePredicate, func);
            return new CypherQueryExecute<TOut>(_cypherEndpoint, query);
        }
    }
}