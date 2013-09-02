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

        [TestMethod]
        public void QueryGraph_SimpleQueryNotInsideTransaction_ReturnsResults()
        {
            var endpoint = CypherEndpointFactory.Create("http://localhost:7474/db/data/");

            var nodes = endpoint.BeginQuery(p => new { node = p.Node })
                    .Start(n => Start.At(n.node, 1874))
                    .Return(r => new { Node = r.node })
                    .Fetch();

            Assert.AreEqual(nodes.Count(), 1);
            Assert.AreEqual(nodes.First().Node.Id, 1874);
        }

        [TestMethod]
        public void QueryWithJoins_NotInsideTransaction_ReturnsResults()
        {
            var endpoint = CypherEndpointFactory.Create("http://localhost:7474/db/data/");

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
            var endpoint = CypherEndpointFactory.Create("http://localhost:7474/db/data/");

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
                var endpoint = CypherEndpointFactory.Create("http://localhost:7474/db/data/");
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
