namespace CypherNet.IntegrationTests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Transactions;

    using CypherNet.Configuration;
    using CypherNet.Graph;
    using CypherNet.Transaction;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IntegrationTests
    {
        private static Node _personNode, _positionNode;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(@"d:\Cypher.Net\TextWriterOutput.log"));
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Trace.Flush();
        }

        [TestMethod]
        public void CanLogin_WithPassword()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            _personNode = endpoint.CreateNode(new { name = "Plzensky Prazdroj" }, "brewery");
            var twin = endpoint.GetNode(_personNode.Id);
            Assert.AreEqual(twin.Id, _personNode.Id);
            Assert.IsTrue(object.ReferenceEquals(_personNode, twin));
        }

        [TestMethod]
        public void CreateNode_ReturnsNewNode()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            _personNode = endpoint.CreateNode(new { name = "Plzensky Prazdroj" }, "brewery");
            var twin = endpoint.GetNode(_personNode.Id);
            Assert.AreEqual(twin.Id, _personNode.Id);
            Assert.IsTrue(object.ReferenceEquals(_personNode, twin));
        }

        [TestMethod]
        public void CreateNode_MultipleLabels_ReturnsNewNode()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            _personNode = endpoint.CreateNode(new { name = "Plzensky Prazdroj" }, "brewery", "czech");
            var twin = endpoint.GetNode(_personNode.Id);
            Assert.AreEqual(twin.Id, _personNode.Id);
            Assert.IsTrue(object.ReferenceEquals(_personNode, twin));
        }

        [TestMethod]
        public void CreateNode_MultipleProperties_ReturnsNewNode()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var newNode = endpoint.CreateNode(new { name = "Plzensky Prazdroj", age = 100 }, "brewery");
            var twin = endpoint.GetNode(newNode.Id);
            Assert.AreEqual(twin.Id, newNode.Id);
            Assert.IsTrue(object.ReferenceEquals(newNode, twin));
        }

        [TestMethod]
        public void CreateNode_NullProperty_ReturnsNewNode()
        {
            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
                var endpoint = clientFactory.Create();

                var newNode = endpoint.CreateNode(new TestNodeType { Name = "Plzensky Prazdroj", Age = 33, Reference = null }, "brewery");
                var twin = endpoint.GetNode(newNode.Id);
                Assert.AreEqual(twin.Id, newNode.Id);
                Assert.IsTrue(object.ReferenceEquals(newNode, twin));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CypherResponseException))]
        public void NonsenseQuery_ThrowsException()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var query =
                endpoint.BeginQuery(c => new {Node = c.Node})
                    .Start(c => c.StartAtId(c.Vars.Node, 1))
                    .Where(c => c.Prop<int>(c.Vars.Node, "aaa sss ddd") == 1)
                    .Return(c => c.Vars.Node)
                    .Fetch();
        }

        [TestMethod]
        public void CreateRelationship_ReturnsRelationship()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var newNode1 = endpoint.CreateNode(new { name = "mark", age = 33 }, "person");
            var newNode2 = endpoint.CreateNode(new { role = "developer"}, "job");
            var rel = endpoint.CreateRelationship(newNode1, newNode2, "WORKS_AS_A");
            Assert.IsNotNull(rel);
            Assert.AreEqual("WORKS_AS_A", rel.Type);
        }

        [TestMethod]
        public void CreateRelationship_WithData_ReturnsRelationship()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var newNode1 = endpoint.CreateNode(new { name = "mark", age = 33 }, "person");
            var newNode2 = endpoint.CreateNode(new { role = "developer" }, "job");
            var rel = endpoint.CreateRelationship(newNode1, newNode2, "WORKS_AS_A", new { created = DateTime.Now.ToString("s") });
            Assert.IsNotNull(rel);
            Assert.AreEqual("WORKS_AS_A", rel.Type);
        }

        [TestMethod]
        public void DeleteRelationship_DeletesRelationship()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var newNode1 = endpoint.CreateNode(new { name = "mark", age = 33 }, "person");
            var newNode2 = endpoint.CreateNode(new { role = "developer" }, "job");
            var rel = endpoint.CreateRelationship(newNode1, newNode2, "WORKS_AS_A");

            endpoint.DeleteRelationship(rel);

            var actual =
                endpoint.BeginQuery(s => new { person = s.Node, worksAs = s.Rel, job = s.Node })
                    .Start(ctx => ctx
                        .StartAtId(ctx.Vars.person, newNode1.Id)
                        .StartAtId(ctx.Vars.job, newNode2.Id))
                        .Match(ctx => ctx.Node(ctx.Vars.person).Outgoing(ctx.Vars.worksAs).To(ctx.Vars.job))
                    .Return(ctx => ctx.Vars.worksAs)
                    .Fetch()
                    .FirstOrDefault();

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void DeleteRelationship_WithinTransaction_Commit_DeletesRelationship()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var newNode1 = endpoint.CreateNode(new { name = "mark", age = 33 }, "person");
            var newNode2 = endpoint.CreateNode(new { role = "developer" }, "job");
            var rel = endpoint.CreateRelationship(newNode1, newNode2, "WORKS_AS_A");

            using (var trans2 = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                endpoint.DeleteRelationship(rel);
                trans2.Complete();
            }

            var actual =
                endpoint.BeginQuery(s => new { person = s.Node, worksAs = s.Rel, job = s.Node })
                    .Start(ctx => ctx.StartAtId(ctx.Vars.person, newNode1.Id).StartAtId(ctx.Vars.job, newNode2.Id))
                    .Match(ctx => ctx.Node(ctx.Vars.person).Outgoing(ctx.Vars.worksAs).To(ctx.Vars.job))
                    .Return(ctx => ctx.Vars.worksAs)
                    .Fetch()
                    .FirstOrDefault();

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void DeleteRelationship_WithinTransaction_Rollback_DoesNotDeleteRelationship()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var newNode1 = endpoint.CreateNode(new { name = "mark", age = 33 }, "person");
            var newNode2 = endpoint.CreateNode(new { role = "developer" }, "job");
            var rel = endpoint.CreateRelationship(newNode1, newNode2, "WORKS_AS_A");

            using (var trans2 = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                endpoint.DeleteRelationship(rel);
            }

            var actual =
                endpoint.BeginQuery(s => new { person = s.Node, worksAs = s.Rel, job = s.Node })
                    .Start(ctx => ctx.StartAtId(ctx.Vars.person, newNode1.Id).StartAtId(ctx.Vars.job, newNode2.Id))
                    .Match(ctx => ctx.Node(ctx.Vars.person).Outgoing(ctx.Vars.worksAs).To(ctx.Vars.job))
                    .Return(ctx => ctx.Vars.worksAs)
                    .Fetch()
                    .FirstOrDefault();

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Id, rel.Id);
        }


        [TestMethod]
        public void DeleteNode_DeletesNode()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();
            var node  = endpoint.CreateNode(new { name = "mark", age = 33 }, "person");
            endpoint.Delete(node);
            var twin = endpoint.GetNode(node.Id);

            Assert.IsNull(twin);
        }

        [TestMethod]
        public void UpdateNode_UpdatesNode()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            dynamic node = endpoint.CreateNode(new { name = "mark", age = 33 }, "person");
            node.name = "john";
            endpoint.Save(node);
            endpoint.Clear();
            dynamic twin = endpoint.GetNode(node.Id);

            Assert.AreEqual(twin.name, "john");
        }

        [TestMethod]
        public void UpdateNode_RollbackTransaction_DoesNotUpdatesNode()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            dynamic node = endpoint.CreateNode(new { name = "mark", age = 33 }, "person");
            node.name = "john";
            using (var ts = new TransactionScope())
            {
                endpoint.Save(node);
            }

            endpoint.Clear();
            dynamic twin = endpoint.GetNode(node.Id);

            Assert.AreNotEqual(twin.name, "john");
        }

        [TestMethod]
        public void CreateNode_WithLabel_ReturnsNewNode()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            _personNode = endpoint.CreateNode(new {name = "mark", age = 33}, "person");
            dynamic node = _personNode;

            Assert.AreEqual(node.name, "mark");
            Assert.AreEqual(node.age, 33);
        }


        [TestMethod]
        public void QueryGraph_ReturnsMultipleInstancesOfANode_ReturnsIdenticallyEqualNodes()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var testNode = endpoint.CreateNode(new { name = "mark", age = 33 }, "person");
            var results = endpoint.BeginQuery(s => new {node1 = s.Node, node2 = s.Node})
                                 .Start(ctx => ctx.StartAtId(ctx.Vars.node1, testNode.Id).StartAtId(ctx.Vars.node2, testNode.Id))
                                 .Return(ctx => new { Node1 = ctx.Vars.node1, Node2 = ctx.Vars.node2 })
                                 .Fetch();

            foreach (var result in results)
            {
                Assert.AreSame(result.Node1, result.Node2);
            }

        }

        [TestMethod]
        public void CreateNode_WithoutLabel_ReturnsNewNode()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            _positionNode = endpoint.CreateNode(new {position = "developer"});

            var newnode = endpoint
                .BeginQuery(s => new {n = s.Node})
                .Start(ctx => ctx.StartAtId(ctx.Vars.n, _positionNode.Id))
                .Return(ctx => new { NewNode = ctx.Vars.n })
                .Fetch().Select(s => s.NewNode).FirstOrDefault();

            Assert.IsNotNull(newnode);
            Assert.IsTrue(_positionNode.Id == newnode.Id);
        }

        [TestMethod]
        public void CreateRelationship_ReturnsResults()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var path = endpoint
                .BeginQuery(s => new {person = s.Node, worksAs = s.Rel, position = s.Node})
                .Start(ctx => ctx
                                .StartAtId(ctx.Vars.person, _personNode.Id)
                                .StartAtId(ctx.Vars.position, _positionNode.Id))
                .Create(ctx => ctx.CreateRel(ctx.Vars.person, ctx.Vars.worksAs, "WORKS_AS", ctx.Vars.position))
                .Return(ctx => new { ctx.Vars.person, ctx.Vars.worksAs, ctx.Vars.position })
                .Fetch()
                .FirstOrDefault();

            Assert.IsNotNull(path);
            dynamic person = path.person;
            dynamic position = path.position;
            Assert.AreEqual("mark WORKS_AS developer", 
                            String.Format("{0} {1} {2}", person.name, path.worksAs.Type, 
                                          position.position));
        }


        [TestMethod]
        public void CreateNodeWithinTransaction_Rollback_DoesNotCreateNode()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            Node node = null;

            using (var trans = new TransactionScope())
            {
                var endpoint = clientFactory.Create();
                node = endpoint.CreateNode(new {name = "mark", age = 33});
            }

            var readEndpoint = clientFactory.Create();
            var newnode = readEndpoint.BeginQuery(s => new {n = s.Node})
                                      .Start(ctx => ctx.StartAtId(ctx.Vars.n, node.Id))
                                      .Return(ctx => new {NewNode = ctx.Vars.n})
                                      .Fetch().FirstOrDefault();

            Assert.IsNull(newnode);
        }

        [TestMethod]
        public void QueryGraph_SimpleQueryNotInsideTransaction_ReturnsResults()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var nodes = endpoint.BeginQuery(p => new {node = p.Node})
                                .Start(ctx => ctx.StartAtId(ctx.Vars.node, _personNode.Id))
                                .Return(ctx => new {Node = ctx.Vars.node})
                                .Fetch();

            Assert.AreEqual(nodes.Count(), 1);
            Assert.AreEqual(nodes.First().Node.Id, _personNode.Id);
        }

        [TestMethod]
        [Ignore]
        public void QueryWithJoinsOverMany_NotInsideTransaction_ReturnsMultipleResults()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var cypherEndpoint = clientFactory.Create();

            var nodes = cypherEndpoint
                .BeginQuery(p => new {person = p.Node, rel = p.Rel, role = p.Node}) // Define query variables
                .Start(ctx => ctx.StartAtAny(ctx.Vars.person)) // Cypher START clause
                .Match(ctx => ctx.Node(ctx.Vars.person).Outgoing(ctx.Vars.rel).To(ctx.Vars.role)) // Cypher MATCH clause
                .Where(ctx =>
                       ctx.Prop<string>(ctx.Vars.person, "name") == "mark" && ctx.Prop<string>(ctx.Vars.role, "title") == "developer")
                // Cypher WHERE predicate
                .Return(ctx => new { Person = ctx.Vars.person, Rel = ctx.Vars.rel, Role = ctx.Vars.role }) // Cypher RETURN clause
                .Fetch(); // GO!

            /* Executes Cypher: 
             * START person:node(*) 
             * MATCH (person)-[rel]->(role) 
             * WHERE person.name! = 'mark' AND role.title! = 'developer' 
             * RETURN person as Person, rel as Rel, role as ROle
            */

            Assert.IsTrue(nodes.Any());

            foreach (var node in nodes)
            {
                dynamic start = node.Person; // Nodes & Relationships are dynamic types
                dynamic end = node.Role;
                Assert.AreEqual("mark", start.name);
                Assert.AreEqual("developer", end.title);
                Console.WriteLine(String.Format("{0} {1} {2}", start.name, node.Rel.Type, end.title));
                    // Prints "mark IS_A developer"
            }
        }

        [TestMethod]
        public void QueryGraph_SimpleQueryInsideTransaction_ReturnsResults()
        {
            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
            {
                var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
                var cypherEndpoint = clientFactory.Create();
                var nodes = cypherEndpoint.BeginQuery(p => new {node = p.Node})
                                          .Start(ctx => ctx.StartAtAny(ctx.Vars.node))
                                          .Where(ctx => ctx.Vars.node.Id == _personNode.Id)
                                          .Return(ctx => new {Node = ctx.Vars.node})
                                          .Fetch();
                Assert.AreEqual(nodes.Count(), 1);
                trans.Complete();
            }
        }

        [TestMethod]
        public void NestedTransactions_CommitInnerRollbackOuter_DoesNotCreateOuterNode()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var cypherEndpoint = clientFactory.Create();
            Node node1, node2;
            using (var trans1 = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
            {
                node1 = cypherEndpoint.CreateNode(new {name = "test node1"});
                using (var trans2 = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    node2 = cypherEndpoint.CreateNode(new {name = "test node2"});
                    trans2.Complete();
                }
            }

            var node1Query = cypherEndpoint.BeginQuery(s => new {node1 = s.Node})
                                           .Start(ctx => ctx.StartAtId(ctx.Vars.node1, node1.Id))
                                           .Return(ctx => new {ctx.Vars.node1})
                                           .Fetch()
                                           .FirstOrDefault();

            var node2Query = cypherEndpoint.BeginQuery(s => new {node2 = s.Node})
                                           .Start(ctx => ctx.StartAtId(ctx.Vars.node2, node2.Id))
                                           .Return(ctx => new { ctx.Vars.node2 })
                                           .Fetch()
                                           .FirstOrDefault();

            Assert.IsNull(node1Query);
            Assert.IsNotNull(node2Query);
        }

        [TestMethod]
        public void QueryGraph_SimpleQueryOnProperty_ReturnsResults()
        {
            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
            {
                var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
                var endpoint = clientFactory.Create();

                dynamic node = endpoint.CreateNode(new { name = "fred", age = 33 }, "person");

                var nodes = endpoint.BeginQuery(p => new { node = p.Node })
                                          .Start(ctx => ctx.StartAtAny(ctx.Vars.node))
                                          .Where(ctx => ctx.Prop<string>(ctx.Vars.node, "name") == "fred")
                                          .Return(ctx => new { Node = ctx.Vars.node })
                                          .Fetch();
                Assert.AreEqual(1, nodes.Count());
                Assert.AreEqual("fred", node.name);
            }
        }

        [TestMethod]
        public void CreateConstraint_DoesNotThrowException()
        {
            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
            {
                var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
                var endpoint = clientFactory.Create();

                var uniqueLabel = "uniqueLabel";

                endpoint.CreateConstraint(uniqueLabel, "name");

                // Cannot modify data in same transacction as schema updates
            }
        }

        [TestMethod]
        public void CreateConstraint_PreventsDuplicates()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            var uniqueLabel = "anotherUniqueLabel";

            try
            {
                endpoint.CreateConstraint(uniqueLabel, "name");

                using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
                {
                    var txEndpoint = clientFactory.Create();
                    txEndpoint.CreateNode(new { name = "fred", age = 33 }, uniqueLabel);
                    txEndpoint.CreateNode(new { name = "fred", age = 33 }, uniqueLabel);
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
                Assert.IsTrue(ex is CypherResponseException);
            }
            finally
            {
                endpoint.DropConstraint(uniqueLabel, "name");
            }
        }

        [TestMethod]
        public void QueryGraph_SimpleQueryHasProperty_ReturnsResults()
        {
            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
            {
                var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
                var endpoint = clientFactory.Create();

                dynamic node = endpoint.CreateNode(new { firstname = "fred", age = 33 }, "person");

                var nodes = endpoint.BeginQuery(p => new { node = p.Node })
                                          .Start(ctx => ctx.StartAtAny(ctx.Vars.node))
                                          .Where(ctx => ctx.Has(ctx.Vars.node, "firstname"))
                                          .Return(ctx => new { Node = ctx.Vars.node })
                                          .Fetch();
                Assert.AreEqual(1, nodes.Count());
                Assert.AreEqual("fred", node.firstname);
            }
        }

        [TestMethod]
        public void QueryGraph_SimpleQueryHasNonsenseProperty_ReturnsResults()
        {
            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
            {
                var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
                var endpoint = clientFactory.Create();

                dynamic node = endpoint.CreateNode(new { name = "fred", age = 33 }, "person");

                var nodes = endpoint.BeginQuery(p => new { node = p.Node })
                                          .Start(ctx => ctx.StartAtAny(ctx.Vars.node))
                                          .Where(ctx => ctx.Has(ctx.Vars.node, "xxxxx"))
                                          .Return(ctx => new { Node = ctx.Vars.node })
                                          .Fetch();
                Assert.AreEqual(0, nodes.Count());
            }
        }

        [TestMethod]
        public void QueryGraph_OptionalMatch_ReturnsResults()
        {
            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
            {
                var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
                var endpoint = clientFactory.Create();

                var acme = endpoint.CreateNode(new { role = "acme co." }, "company");

                var frank = endpoint.CreateNode(new { name = "frank", age = 35 }, "person");
                var rel2 = endpoint.CreateRelationship(frank, acme, "WORKS_FOR");

                var james = endpoint.CreateNode(new { name = "james", age = 25 }, "person");
                var rel3 = endpoint.CreateRelationship(james, acme, "WORKS_FOR");

                var nodes =
                    endpoint.BeginQuery(p => new { person = p.Node, company = p.Node, x = p.Node })
                        .Start(ctx => ctx.StartAtId(ctx.Vars.person, frank.Id))
                        .Match(ctx => ctx.Node(ctx.Vars.person).Outgoing("WORKS_FOR").To(ctx.Vars.company))
                        .OptionalMatch(ctx => ctx.Node(ctx.Vars.company).Incoming().From(ctx.Vars.x))
                        .Return(ctx => new { ctx.Vars.x })
                        .Fetch();

                Assert.AreEqual(2, nodes.Count());
                Assert.AreEqual(nodes.First().x.Id, frank.Id);
                Assert.AreEqual(nodes.Last().x.Id, james.Id);
            }
        }

        [TestMethod]
        public void UpdateCompleteTransaction_UpdateIsPersisted()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            const int originalValue = 100;
            const int newValue = 200;

            var personNode = endpoint.CreateNode(new { name = "Plzensky Prazdroj", age = originalValue });

            using (var transaction = new TransactionScope())
            {
                dynamic node = endpoint.GetNode(personNode.Id);

                node.age = newValue;
                endpoint.Save(node);

                transaction.Complete();

            }

            dynamic actualPerson = endpoint.GetNode(personNode.Id);
            Assert.AreEqual(newValue, actualPerson.age);
        }

        [TestMethod]
        public void UpdateRollback_EnsureEverythingIsRolledBack()
        {
            var clientFactory = Fluently.Configure("server=http://localhost:7474/db/data/;User Id=neo4j;password=password").CreateSessionFactory();
            var endpoint = clientFactory.Create();

            const int originalValue = 100;
            const int newValue = 200;

            var personNode = endpoint.CreateNode(new { name = "Plzensky Prazdroj", age = originalValue });

            using (var transaction = new TransactionScope())
            {
                dynamic node = endpoint.GetNode(personNode.Id);

                node.age = newValue;
                endpoint.Save(node);

            }

            dynamic actualPerson = endpoint.GetNode(personNode.Id);
            Assert.AreEqual(originalValue, actualPerson.age);
        }
    }
}