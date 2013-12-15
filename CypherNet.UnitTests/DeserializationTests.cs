namespace CypherNet.UnitTests
{
    #region

    using System;
    using System.Linq;
    using Graph;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Queries;
    using Serialization;

    #endregion

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
            ""Actor__Labels"",
            ""ActedIn"",
            ""ActedIn__Id"",
            ""ActedIn__Type"",
            ""Movie"",
            ""Movie__Id"",
            ""Movie__Labels""
         ],
         ""data"":[{ ""row"":
            [
               {
                  ""age"":33,
                  ""name"":""mark""
               },
               3745,
                [""person""],
               {

               },
               39490,
               ""IS_A"",
               {
                  ""title"":""developer""
               },
               3746,
               []
            ]},{""row"":[
               {
                  ""age"":21,
                  ""name"":""John""
               },
               3747,
               [""person""],
               {

               },
               39491,
               ""IS_A"",
               {
                  ""title"":""leg""
               },
               3748,
               []
            ]}
         ]
      }
   ],
   ""transaction"":{
      ""expires"":""Tue, 30 Jul 2013 15:57:59 +0000""
   },
   ""errors"":[

   ]
}";

            var deserializer = new DefaultJsonSerializer(new DictionaryEntityCache());

            var retval = deserializer.Deserialize<CypherResponse<DeserializationTestClass>>(json);
            Assert.AreEqual(retval.Results.Count(), 2);
            dynamic actor = retval.Results.Select(r => r.Actor).First();
            Assert.AreEqual(actor.age, 33);
            Assert.AreEqual(actor.name, "mark");
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