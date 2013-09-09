CypherNet
=========

A .Net API for the Neo4j HTTP Transactional Endpoint. (v2.0.0-M04)

Exposes strongly typed, lambda expression based Graph Query API based on the Neo4j [Cypher Query Language](http://docs.neo4j.org/chunked/milestone/cypher-query-lang.html).


<dl>
    <dt>Usage</dt>
    <dd></dd>
</dl>
```C#
  var nodes = cypherEndpoint
          .BeginQuery(p => new { person = p.Node, rel = p.Rel, role = p.Node }) // Define query variables
          .Start(vars => Start.Any(vars.person)) // Cypher START clause
          .Match(vars => Pattern.Start(vars.person).Outgoing(vars.rel).To(vars.role))  // Cypher MATCH clause
          .Where(vars => vars.person.Get<string>("name!") == "mark" && vars.role.Get<string>("title!") == "developer") // Cypher WHERE predicate
          .Return(vars => new { Person = vars.person, Rel = vars.rel, Role = vars.role }) // Cypher RETURN clause
          .Fetch();  // GO!

  Assert.IsTrue(nodes.Any());

  foreach (var node in nodes)
  {
      dynamic start = node.Person;  // Nodes & Relationships are dynamic types
      dynamic end = node.Role;
      Assert.AreEqual("mark", start.name);
      Assert.AreEqual("developer", end.title);
      Console.WriteLine(String.Format("{0} {1} {2}", start.name, node.Rel.Type, end.title)); // Prints "mark IS_A developer"
  }
```
<dl>
    <dt>Transactional</dt>
    <dd></dd>
</dl>
```C#
    using (var trans1 = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromDays(1)))
    {
        var node1 = cypherEndpoint.CreateNode(new {name = "test node1"});
        using (var trans2 = new TransactionScope(TransactionScopeOption.RequiresNew))
        {
            var node2 = cypherEndpoint.CreateNode(new { name = "test node2" });
            trans2.Complete();
        }
    }
    
    var node1Query = cypherEndpoint.BeginQuery(s => new {node1 = s.Node})
                             .Start(s => Start.Any(s.node1))
                             .Where(n => n.node1.Get<string>("name") == "test node1")
                             .Return(r => new {r.node1})
                             .Fetch()
                             .FirstOrDefault();
    
    var node2Query = cypherEndpoint.BeginQuery(s => new {node1 = s.Node})
                             .Start(s => Start.Any(s.node1))
                             .Where(n => n.node1.Get<string>("name!") == "test node2")
                             .Return(r => new {r.node1})
                             .Fetch()
                             .FirstOrDefault();
    
    Assert.IsNull(node1Query);
    Assert.IsNotNull(node2Query);
```
