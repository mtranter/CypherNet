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
    public class DeserializationTests
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
