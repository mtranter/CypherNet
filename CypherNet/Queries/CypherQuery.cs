namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Graph;

    #endregion

    internal static class CypherQuery
    {
        internal static readonly Type[] ValidGraphEntityTypes = new[] {typeof (IGraphEntity)};
    }

    internal class FluentCypherQueryBuilder<TIn> : ICypherQueryStart<TIn>, ICypherQueryMatch<TIn>,
                                                   ICypherQueryWhere<TIn>, ICypherQueryReturns<TIn>, ICypherQuerySetable<TIn>
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

        public ICypherQueryReturns<TIn> Set(params Expression<Action<TIn>>[] setters)
        {
            _setters = setters;
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

        public ICypherOrderBy<TIn,TOut> Return<TOut>(Expression<Func<TIn, TOut>> func)
        {

            var query = new CypherQueryDefinition<TIn, TOut>()
                            {
                                StartClause = _startDef,
                                WherePredicate = _wherePredicate,
                                ReturnClause = func
                            };
            foreach (var m in _matchClauses ?? Enumerable.Empty<Expression<Func<TIn, IDefineCypherRelationship>>>())
            {
                query.AddMatchClause(m);
            }
            foreach (var m in _setters ?? Enumerable.Empty<Expression<Action<TIn>>>())
            {
                query.AddSetClause(m);
            }

            return new CypherQueryExecute<TOut>(_cypherEndpoint, _queryBuilder, query);
        }

        internal class CypherQueryExecute<TOut> : ICypherOrderBy<TIn,TOut>
        {
            private readonly ICypher _cypherEndpoint;
            private readonly ICypherQueryBuilder _builder;
            private readonly CypherQueryDefinition<TIn, TOut> _query;
            
            internal CypherQueryExecute(ICypher cypherEndpoint, ICypherQueryBuilder builder, CypherQueryDefinition<TIn,TOut> query)
            {
                _cypherEndpoint = cypherEndpoint;
                _builder = builder;
                _query = query;
            }

            #region ICypherExecute<TOut> Members

            public IEnumerable<TOut> Execute()
            {
                var cypherQuery = _builder.BuildQueryString(_query);
                return _cypherEndpoint.ExecuteQuery<TOut>(cypherQuery);
            }

            #endregion

            public ICypherSkip<TIn, TOut> OrderBy(params Expression<Func<TIn, dynamic>>[] orderBy)
            {
                foreach (var clause in orderBy)
                {
                    _query.AddOrderByClause(clause);
                }

                return this;
            }

            public ICypherExecuteable<TOut> Limit(int limit)
            {
                _query.Limit = limit;
                return this;
            }

            public ICypherLimit<TIn, TOut> Skip(int skip)
            {
                _query.Skip = skip;
                return this;
            }
        }

        public Expression<Action<TIn>>[] _setters { get; set; }
    }
}