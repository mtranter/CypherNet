using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CypherNet.Graph;
using CypherNet.Queries;
using CypherNet.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace CypherNet.UnitTests
{
    [TestClass]
    public class SerializationTests
    {

        [TestMethod]
        public void DerializeJson_EntitiesOnly_ReturnsCollectionOfEntities()
        {
            const string json =
                @"{
   ""commit"":""http://localhost:7474/db/data/transaction/6/commit"",
   ""results"":[
      {
         ""columns"":[
            ""Actor"",
            ""Actor__Id"",
            ""ActedIn"",
            ""ActedIn__Id"",
            ""ActedIn__Type"",
            ""Movie"",
            ""Movie__Id""
         ],
         ""data"":[
            [
               {
                  ""age"":33,
                  ""name"":""mark""
               },
               3745,
               {

               },
               39490,
               ""IS_A"",
               {
                  ""title"":""developer""
               },
               3746
            ],[
               {
                  ""age"":21,
                  ""name"":""John""
               },
               3747,
               {

               },
               39491,
               ""IS_A"",
               {
                  ""title"":""leg""
               },
               3748
            ]
         ]
      }
   ],
   ""transaction"":{
      ""expires"":""Tue, 30 Jul 2013 15:57:59 +0000""
   },
   ""errors"":[

   ]
}";

            var deserializer = new DefaultJsonSerializer();

            var retval = deserializer.Deserialize<CypherResponse<DeserializationTestClass>>(json);
            Assert.AreEqual(retval.Results.Count(),2);
            dynamic actor = retval.Results.Select(r => r.Actor).First();
            Assert.AreEqual(actor.age,33);
            Assert.AreEqual(actor.name, "mark");
        }

        [TestMethod]
        public void SerializeCypherRequest__IsValid()
        {
            var request = CypherQueryRequest.Create(@"START x=node(1), y=node(2) CREATE x-[r:OWNS {""name"":""mark""}]->y<-[r2:IS_OWNED_BY {""age"": 33}]");
            var serializer = new DefaultJsonSerializer();
            var result = serializer.Serialize(request);
            Console.WriteLine(result);
        }
    }

    public class DeserializationTestClass
    {
        public DeserializationTestClass(Node actor, Relationship actedIn, Node movie)
        {
            Actor = actor;
            ActedIn = actedIn;
            Movie = movie;
        }

        public Node Actor { get; set; }
        public Relationship ActedIn { get; set; }
        public Node Movie { get; set; }
    }
}
