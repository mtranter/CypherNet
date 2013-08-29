namespace CypherNet.Queries
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

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
            var createRel = queryDefinition.CreateRelationpClause == null ? null : "CREATE " + BuildCreateRelationshipClause(queryDefinition.CreateRelationpClause);
            var where = queryDefinition.WherePredicate == null ? null : "WHERE " + BuildWhereClause(queryDefinition.WherePredicate);
            var setClause = queryDefinition.SetterClauses.Any() ? "SET " + String.Join(" SET ",queryDefinition.SetterClauses.Select(BuildSetClause)) : null;
            var orderBy = queryDefinition.OrderByClauses.Any() ? "ORDER BY " + String.Join(", ", queryDefinition.OrderByClauses.Select(BuildOrderByClause)) : null;
            
            var @return = "RETURN " + BuildReturnClause(queryDefinition.ReturnClause);
            var skip = queryDefinition.Skip == null ? null : String.Format("SKIP {0}", queryDefinition.Skip);
            var limit = queryDefinition.Limit == null ? null : String.Format("LIMIT {0}", queryDefinition.Limit);
            return String.Join(" ", new[] { start, createRel, match, where, setClause, @return, orderBy, skip, limit }.Where(s => s != null));
        }

        internal string BuildStartClause(Expression exp)
        {
            return CypherStartClauseBuilder.BuildStartClause(exp);
        }

        internal string BuildCreateRelationshipClause(Expression exp)
        {
            return CypherCreateRelationshipClauseBuilder.BuildCreateClause(exp);
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