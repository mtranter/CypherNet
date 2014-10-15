namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Transaction;

    #endregion

    internal class FluentCypherQueryBuilder<TIn> : ICypherQueryStart<TIn>, ICypherQueryMatch<TIn>,
                                                   ICypherQueryWhere<TIn>, ICypherQueryReturns<TIn>,
                                                   ICypherQuerySetable<TIn>, ICypherQueryCreate<TIn>

    {
        private readonly ICypherClientFactory _clientFactory;
        private readonly string _cypherQuery = String.Empty;
        private Expression<Func<ICreateRelationshipQueryContext<TIn>, ICreateCypherRelationship>> _createClause;
        private Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>[] _matchClauses;
        private Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>[] _optionalMatchClauses;
        private Expression<Func<IUpdateQueryContext<TIn>, ISetResult>>[] _setters;
        private Expression<Action<IStartQueryContext<TIn>>> _startDef;
        private Expression<Func<IWhereQueryContext<TIn>, bool>> _wherePredicate;

        internal FluentCypherQueryBuilder(ICypherClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        internal string CypherQuery
        {
            get { return _cypherQuery; }
        }

        public ICypherQueryReturns<TIn> Create(Expression<Func<ICreateRelationshipQueryContext<TIn>, ICreateCypherRelationship>> createClause)
        {
            _createClause = createClause;
            return this;
        }

        public ICypherQueryReturnOrExecute<TIn> Update(params Expression<Func<IUpdateQueryContext<TIn>, ISetResult>>[] setters)
        {
            _setters = setters;
            var query = BuildCypherQueryDefinition<TIn>();
            return new CypherQueryExecute<TIn>(_clientFactory, query);
        }

        public ICypherQueryReturns<TIn> Where(Expression<Func<IWhereQueryContext<TIn>, bool>> predicate)
        {
            _wherePredicate = predicate;
            return this;
        }

        public ICypherOrderBy<TIn, TOut> Return<TOut>(Expression<Func<IReturnQueryContext<TIn>, TOut>> func)
        {
            var query = BuildCypherQueryDefinition<TOut>();
            query.ReturnClause = func;
            return new CypherQueryExecute<TOut>(_clientFactory, query);
        }

        private CypherQueryDefinition<TIn,TOut> BuildCypherQueryDefinition<TOut>()
        {
            var query = new CypherQueryDefinition<TIn, TOut>
                            {
                                StartClause = _startDef,
                                WherePredicate = _wherePredicate,
                                CreateRelationpClause = _createClause
                            };
            foreach (var m in _matchClauses ?? Enumerable.Empty<Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>>())
            {
                query.AddMatchClause(m);
            }
            foreach (var m in _optionalMatchClauses ?? Enumerable.Empty<Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>>())
            {
                query.AddOptionalMatchClause(m);
            }
            foreach (var m in _setters ?? Enumerable.Empty<Expression<Func<IUpdateQueryContext<TIn>, ISetResult>>>())
            {
                query.AddSetClause(m);
            }
            return query;
        }

        public ICypherQueryMatch<TIn> Start(Expression<Action<IStartQueryContext<TIn>>> startDef)
        {
            _startDef = startDef;
            return this;
        }

        public ICypherQueryOptionalMatchOnly<TIn> Match(params Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>[] matchDefs)
        {
            _matchClauses = matchDefs;
            return this;
        }

        public ICypherQueryWhere<TIn> OptionalMatch(params Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>[] matchDefs)
        {
            _optionalMatchClauses = matchDefs;
            return this;
        }

        public ICypherExecuteable Delete<TOut>(Expression<Func<TIn, TOut>> deleteClause)
        {
            var query = new CypherQueryDefinition<TIn, TOut>
            {
                StartClause = _startDef,
                WherePredicate = _wherePredicate,
                DeleteClause = deleteClause,
                CreateRelationpClause = _createClause
            };
            foreach (var m in _matchClauses ?? Enumerable.Empty<Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>>())
            {
                query.AddMatchClause(m);
            }
            foreach (var m in _optionalMatchClauses ?? Enumerable.Empty<Expression<Func<IMatchQueryContext<TIn>, IDefineCypherRelationship>>>())
            {
                query.AddOptionalMatchClause(m);
            }
            foreach (var m in _setters ?? Enumerable.Empty<Expression<Func<IUpdateQueryContext<TIn>, ISetResult>>>())
            {
                query.AddSetClause(m);
            }

            return new CypherQueryExecute<TOut>(_clientFactory, query);
        }

        internal class CypherQueryExecute<TOut> : ICypherOrderBy<TIn, TOut>, ICypherQueryReturnOrExecute<TIn>
        {
            private readonly ICypherClientFactory _clientFactory;
            private readonly CypherQueryDefinition<TIn, TOut> _query;

            internal CypherQueryExecute(ICypherClientFactory clientFactory, CypherQueryDefinition<TIn, TOut> query)
            {
                _clientFactory = clientFactory;
                _query = query;
            }

            public ICypherSkip<TIn, TOut> OrderBy(params Expression<Func<TIn, dynamic>>[] orderBy)
            {
                foreach (var clause in orderBy)
                {
                    _query.AddOrderByClause(clause);
                }

                return this;
            }

            public ICypherFetchable<TOut> Limit(int limit)
            {
                _query.Limit = limit;
                return this;
            }

            public ICypherLimit<TIn, TOut> Skip(int skip)
            {
                _query.Skip = skip;
                return this;
            }

            public IEnumerable<TOut> Fetch()
            {
                var cypherQuery = _query.BuildStatement();
                var client = _clientFactory.Create();
                return client.ExecuteQuery<TOut>(cypherQuery);
            }

            public ICypherOrderBy<TIn, TOut1> Return<TOut1>(Expression<Func<IReturnQueryContext<TIn>, TOut1>> returnsClause)
            {
                var query = _query.Copy<TOut1>();
                query.ReturnClause = returnsClause;
                return new CypherQueryExecute<TOut1>(_clientFactory, query);
            }

            void ICypherExecuteable.Execute()
            {
                var cypherQuery = _query.BuildStatement();
                var client = _clientFactory.Create();
                client.ExecuteCommand(cypherQuery);
            }

        }
    }
}