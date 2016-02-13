
CypherNet
=========
[![Gitter](https://badges.gitter.im/Join Chat.svg)](https://gitter.im/mtranter/CypherNet?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/mtpg771qhljc3jai?svg=true)](https://ci.appveyor.com/project/mtranter/cyphernet)

A .Net API for the Neo4j HTTP Transactional Endpoint. (v2.0.0)

Exposes strongly typed Graph Query API based on the Neo4j [Cypher Query Language](http://docs.neo4j.org/chunked/milestone/cypher-query-lang.html).

<dl>
    <dt>Connection String</dt>
    <dd></dd>
</dl>
```C#
var clientFactory = Fluently.Configure("Server=http://localhost:7474/db/data/;User Id=neo4j;Password=password").CreateSessionFactory();
var cypherEndpoint = clientFactory.Create();
```
or for Unauthd:
```C#
var clientFactory = Fluently.Configure("http://localhost:7474/db/data/").CreateSessionFactory();
var cypherEndpoint = clientFactory.Create();
```
<dl>
    <dt>Usage</dt>
    <dd></dd>
</dl>
```C#
var clientFactory = Fluently.Configure("http://localhost:7474/db/data/").CreateSessionFactory();
var cypherEndpoint = clientFactory.Create();

var nodes = cypherEndpoint
    .BeginQuery(p => new {person = p.Node, rel = p.Rel, role = p.Node}) // Define query variables
    .Start(ctx => ctx.StartAtAny(ctx.Vars.person)) // Cypher START clause
    .Match(ctx => ctx.Node(ctx.Vars.person).Outgoing(ctx.Vars.rel).To(ctx.Vars.role)) // Cypher MATCH clause
    .Where(ctx =>
           ctx.Prop<string>(ctx.Vars.person, "name!") == "mark" && ctx.Prop<string>(ctx.Vars.role, "title!") == "developer")
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
```
<dl>
    <dt>Transactional</dt>
    <dd></dd>
</dl>
```C#
var clientFactory = Fluently.Configure("http://localhost:7474/db/data/").CreateSessionFactory();
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
```


Me: [http://www.mtranter.com](http://www.mtranter.com)
