using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using CypherNet.Configuration;
using CypherNet.Dynamic;
using CypherNet.Graph;
using CypherNet.Http;
using CypherNet.Transaction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CypherNet.UnitTests
{
    [TestClass]
    public class CypherSessionTransactionTests
    {
        private const string BaseUri = "http://localhost:7474/db/data/";
        private const string AutoCommitAddress = "http://localhost:7474/db/data/transaction/commit";
        private const string BeginTransactionUri = "http://localhost:7474/db/data/transaction/";
        private const string KeepAliveAddress = "http://localhost:7474/db/data/transaction/1";
        private const string CommitAddress = "http://localhost:7474/db/data/transaction/1/commit";
        private const string EmptyRequest = @"{""statements"":[]}";
        private const string EmptyResponse = @"{""results"":[],""errors"":[]}";
        private const string Name = "Marcus.T.Peeps";
        private static readonly object Node = new {name = Name};

        [TestMethod]
        public void CreateNode__CreatesNode()
        {
            var mock = InitializeMockWebClient(AutoCommitAddress);

            var session = new CypherSession(new ConnectionProperties(BaseUri), mock.Object);
            var node = session.CreateNode(new {name = Name}, "person");
            Assert.AreEqual(node.AsDynamic().name, Name);
            Assert.AreEqual((node).Labels.First(), "person");
        }
        
        [TestMethod]
        public void CreateNode_WithinCommitedTransaction_CallsCommit()
        {
            var mock = InitializeMockWebClient(BeginTransactionUri);

            //Keep alive.
            mock.Setup(m => m.PostAsync(KeepAliveAddress, EmptyRequest))
                .Returns(() => BuildResponse(@"{""commit"":""" + CommitAddress + @""",""results"":[],""transaction"":{""expires"":""Wed, 02 Oct 2013 15:18:27 +0000""},""errors"":[]}"));
            
            //Commit
            mock.Setup(m => m.PostAsync(CommitAddress, EmptyRequest)).Returns(() => BuildResponse(EmptyResponse));

            var session = new CypherSession(new ConnectionProperties(BaseUri), mock.Object);
            using (var ts = new TransactionScope())
            {
                session.CreateNode(new {name = Name}, "person");
                ts.Complete();
            }

            mock.Verify(m => m.PostAsync(KeepAliveAddress, EmptyRequest));
            mock.Verify(m => m.PostAsync(CommitAddress, EmptyRequest));
        }
        
        [TestMethod]
        public void CreateNode_WithinRollbackTransaction_CallsRollback()
        {
            var mock = InitializeMockWebClient(BeginTransactionUri);

            //Rollback
            mock.Setup(m => m.DeleteAsync(KeepAliveAddress)).Returns(() => BuildResponse(EmptyResponse));

            var session = new CypherSession(new ConnectionProperties(BaseUri), mock.Object);
            
            using (new TransactionScope())
            {
                session.CreateNode(new { name = Name }, "person");
            }

            mock.Verify(m => m.DeleteAsync(KeepAliveAddress));
        }

        private Mock<IWebClient> InitializeMockWebClient(string uri)
        {
            var mock = new Mock<IWebClient>();

            mock.Setup(
                m =>
                m.PostAsync(uri,
                            @"{""statements"":[{""statement"":""CREATE (NewNode:person {param_0}) RETURN NewNode as NewNode, id(NewNode) as NewNode__Id, labels(NewNode) as NewNode__Labels;"",""parameters"":{""param_0"":{""name"":""" +
                            Name + @"""}}}]}"))
                .Returns(
                    () =>
                    BuildResponse(@"{""commit"":""" + CommitAddress +
                                  @""",""results"":[{""columns"":[""NewNode"",""NewNode__Id"",""NewNode__Labels""],""data"":[{""row"": [{""name"":""" + Name + @"""},15026,[""person""]]}]}],""errors"":[]}"));
            return mock;
        }

        private Task<IHttpResponseMessage> BuildResponse(string response)
        {
            return Task.FromResult((IHttpResponseMessage) new MockHttpResponseMessage(response, HttpStatusCode.OK));
        }
    }
}

