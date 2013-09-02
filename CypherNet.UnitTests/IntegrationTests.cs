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
        public void QueryGraph_SimpleQuery_ReturnsResults()
        {
            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
            {
                var endpoint = CypherEndpointFactory.Create("http://localhost:7474/db/data/");
                var nodes = endpoint.BeginQuery(() => new {node = new Node(1, null)})
                        .Start(n => Start.At(n.node, 1874))
                        .Return(r => new {Node = r.node })
                        .Fetch();
                Assert.AreEqual(nodes.Count(), 1);
                trans.Complete();
            }
        }
    }
}
