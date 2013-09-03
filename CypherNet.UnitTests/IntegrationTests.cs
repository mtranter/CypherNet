using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using CypherNet.Graph;
using CypherNet.Queries;
using CypherNet.Transaction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CypherNet.UnitTests
{
    [TestClass]
    public class IntegrationTests
    {
        private static Node _developerPersonNode, _roleNode, _positionNode;

        [TestMethod]
        public void CreateNode_WithLabel_ReturnsNewNode()
        {
            var clientFactory = new CypherClientFactory("http://localhost:7474/db/data/");
            var endpoint = new CypherEndpoint(clientFactory);

            _developerPersonNode = endpoint.CreateNode(new { name = "mark", age = 33 }, "person");
            dynamic node = _developerPersonNode;

            Assert.AreEqual(node.name, "mark");
            Assert.AreEqual(node.age, 33);
        }

        [TestMethod]
        public void CreateNode_WithoutLabel_ReturnsNewNode()
        {
            var clientFactory = new CypherClientFactory("http://localhost:7474/db/data/");
            var endpoint = new CypherEndpoint(clientFactory);

            _roleNode = endpoint.CreateNode(new { role = "developer"});

            var newnode = endpoint
                        .BeginQuery(s => new { n = s.Node })
                        .Start(v => Start.At(v.n, _roleNode.Id))
                        .Return(r => new { NewNode = r.n })
                        .Fetch().Select(s => s.NewNode).FirstOrDefault();

            Assert.IsNotNull(newnode);
            Assert.IsTrue(_roleNode.Id == newnode.Id);
            
        }


        [TestMethod]
        public void CreateNodeWithinTransaction_Commit_QueryFindsNewNode()
        {
            var clientFactory = new CypherClientFactory("http://localhost:7474/db/data/");
    
            using (var trans = new TransactionScope())
            {
                var endpoint = new CypherEndpoint(clientFactory);
                _positionNode = endpoint.CreateNode(new { job = "Neo4J Analyst" });
                trans.Complete();
            }

            var readEndpoint = new CypherEndpoint(clientFactory);
            var newnode = readEndpoint
                        .BeginQuery(s => new { n = s.Node })
                        .Start(v => Start.At(v.n, _positionNode.Id))
                        .Return(r => new { NewNode = r.n })
                        .Fetch().FirstOrDefault();

            Assert.IsNotNull(newnode);
            Assert.AreEqual(newnode.NewNode.Id, _positionNode.Id);
        }


        [TestMethod]
        public void CreateNodeWithinTransaction_Rollback_DoesNotCreateNode()
        {
            var clientFactory = new CypherClientFactory("http://localhost:7474/db/data/");
            Node node = null;

            using (var trans = new TransactionScope())
            {
                var endpoint = new CypherEndpoint(clientFactory);
                node = endpoint.CreateNode(new { name = "mark", age = 33 });
            }

            var readEndpoint = new CypherEndpoint(clientFactory);
            var newnode = readEndpoint.BeginQuery(s => new {n = s.Node})
                        .Start(v => Start.At(v.n, node.Id))
                        .Return(r => new {NewNode = r.n})
                        .Fetch().FirstOrDefault();

            Assert.IsNull(newnode);

        }

        [TestMethod]
        public void QueryGraph_SimpleQueryNotInsideTransaction_ReturnsResults()
        {
            var clientFactory = new CypherClientFactory("http://localhost:7474/db/data/");
            var endpoint = new CypherEndpoint(clientFactory);

            var nodes = endpoint.BeginQuery(p => new { node = p.Node })
                    .Start(n => Start.At(n.node, _developerPersonNode.Id))
                    .Return(r => new { Node = r.node })
                    .Fetch();

            Assert.AreEqual(nodes.Count(), 1);
            Assert.AreEqual(nodes.First().Node.Id, _developerPersonNode.Id);
        }

        [TestMethod]
        public void QueryWithJoins_NotInsideTransaction_ReturnsResults()
        {
            var clientFactory = new CypherClientFactory("http://localhost:7474/db/data/");
            var endpoint = new CypherEndpoint(clientFactory);

            var nodes = endpoint.BeginQuery(p => new { mystart = p.Node, rel = p.Rel, end= p.Node })
                    .Start(n => Start.At(n.mystart, 1873))
                    .Match(v => Pattern.Start(v.mystart).Outgoing(v.rel).To(v.end))
                    .Return(r => new { MyStart = r.mystart, Rel = r.rel, End = r.end })
                    .Fetch();

            Assert.AreEqual(nodes.Count(), 1);
            var first = nodes.First();
            Assert.AreEqual(first.MyStart.Id, 1873);
            Assert.AreEqual(first.End.Id, 1872);
            Assert.AreEqual(first.Rel.Id, 35905);
        }


        [TestMethod]
        public void QueryWithJoinsOverMany_NotInsideTransaction_ReturnsMultipleResults()
        {
            var clientFactory = new CypherClientFactory("http://localhost:7474/db/data/");
            var endpoint = new CypherEndpoint(clientFactory);

            var nodes = endpoint
                    .BeginQuery(p => new { mystart = p.Node, rel = p.Rel, end = p.Node })
                    .Match(v => Pattern.Start(v.mystart).Outgoing(v.rel).To(v.end))
                    .Where(v => v.mystart.Get<string>("name!") == "mark" && v.end.Get<string>("title!") == "developer")
                    .Return(r => new { MyStart = r.mystart, Rel = r.rel, End = r.end })
                    .Fetch();


            Assert.IsTrue(nodes.Any());
            foreach (var node in nodes)
            {
                dynamic start = node.MyStart;
                dynamic end = node.End;
                Assert.AreEqual("mark", start.name);
                Assert.AreEqual("developer", end.title);
                Console.WriteLine(String.Format("{0} {1} {2}", start.name, node.Rel.Type, end.title));
            }
        }

        [TestMethod]
        public void QueryGraph_SimpleQueryInsideTransaction_ReturnsResults()
        {
            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
            {
                var clientFactory = new CypherClientFactory("http://localhost:7474/db/data/");
                var endpoint = new CypherEndpoint(clientFactory);
                var nodes = endpoint.BeginQuery(p => new {node = p.Node})
                        .Start(n => Start.At(n.node, 1874))
                        .Return(r => new {Node = r.node })
                        .Fetch();
                Assert.AreEqual(nodes.Count(), 1);
                trans.Complete();
            }
        }
    }
}
