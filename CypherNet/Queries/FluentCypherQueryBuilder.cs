namespace CypherNet.Queries
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Graph;
    using Transaction;

    #endregion

    internal class FluentCypherQueryBuilder<TIn> : ICypherQueryStart<TIn>, ICypherQueryMatch<TIn>,
                                                   ICypherQueryWhere<TIn>, ICypherQueryReturns<TIn>,
                                                   ICypherQuerySetable<TIn>, ICypherQueryCreate<TIn>
    {
        private readonly ICypherClientFactory _clientFactory;
        private readonly string _cypherQuery = String.Empty;
        private Expression<Func<TIn, IDefineCypherRelationship>>[] _matchClauses;
        private Expression<Action<TIn>> _startDef;
        private Expression<Func<TIn, bool>> _wherePredicate;
        private Expression<Action<TIn>>[] _setters;
        private Expression<Func<TIn, ICreateCypherRelationship>> _createClause;

        internal FluentCypherQueryBuilder(ICypherClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
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

        public ICypherQueryReturns<TIn> Create(Expression<Func<TIn, ICreateCypherRelationship>> createClause)
        {
            _createClause = createClause;
            return this;
        }

        public ICypherQueryReturns<TIn> Update(params Expression<Action<TIn>>[] setters)
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
                                ReturnClause = func,
                                CreateRelationpClause = _createClause
                            };
            foreach (var m in _matchClauses ?? Enumerable.Empty<Expression<Func<TIn, IDefineCypherRelationship>>>())
            {
                query.AddMatchClause(m);
            }
            foreach (var m in _setters ?? Enumerable.Empty<Expression<Action<TIn>>>())
            {
                query.AddSetClause(m);
            }

            return new CypherQueryExecute<TOut>(_clientFactory, query);
        }

        internal class CypherQueryExecute<TOut> : ICypherOrderBy<TIn,TOut>
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

            void ICypherExecuteable.Execute()
            {
                var cypherQuery = _query.BuildStatement();
                var client = _clientFactory.Create();
                client.ExecuteCommand(cypherQuery);
            }
        }
    }
}