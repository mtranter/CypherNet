namespace CypherNet.UnitTests
{
    #region

    using System.Linq;
    using CypherNet.Queries;
    using Dynamic4Neo.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    #endregion

    [TestClass]
    public class CypherWriteIntegrationTests
    {

        [TestMethod]
        public void CreateNodeWithCypherQuery_ValidQuery_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypher>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(cypher.Object, new TransactionEndpointCypherQueryBuilder());
            var results = query
                .Start(v => Start.At(v.movie, 1))
                .Match(v => Pattern.Start(v.movie).Incoming("STARED_IN", 1, 5).From(v.actor))
                .Return(v => new {v.actor, v.movie})
                .Fetch();
            
            VerifyCypher(cypher, results.FirstOrDefault(), "START movie=node(1) MATCH (movie)<-[:STARED_IN*1..5]-(actor) RETURN actor as actor, movie as movie");
        }

        void VerifyCypher<TResult>(Mock<ICypher> mock, TResult proto, string query)
        {
            mock.Verify(
              c =>
              c.ExecuteQuery<TResult>(query));
        }
    }
}