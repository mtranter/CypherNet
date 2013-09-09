CypherNet
=========

A .Net API for the Neo4j HTTP Transactional Endpoint. (v2.0.0-M04)

Exposes strongly typed Graph Query API based on the Neo4j [Cypher Query Language](http://docs.neo4j.org/chunked/milestone/cypher-query-lang.html).


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
var nodes = cypherEndpoint
        .BeginQuery(p => new { person = p.Node, rel = p.Rel, role = p.Node }) // Define query variables
        .Start(vars => Start.Any(vars.person)) // Cypher START clause
        .Match(vars => Pattern.Start(vars.person).Outgoing(vars.rel).To(vars.role))  // Cypher MATCH clause
        .Where(vars => vars.person.Get<string>("name!") == "mark" && vars.role.Get<string>("title!") == "developer") // Cypher WHERE predicate
        .Return(vars => new { Person = vars.person, Rel = vars.rel, Role = vars.role }) // Cypher RETURN clause
        .Fetch();

/* Executes Cypher: 
 * START person:node(*) 
 * MATCH (person)-[rel]->(role) 
 * WHERE person.name! = 'mark' AND role.title! = 'developer' 
 * RETURN person as Person, rel as Rel, role as ROle
*/

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
