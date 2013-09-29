

using Newtonsoft.Json;

namespace CypherNet
{
   
    internal class ServiceRootResponse
    {
        [JsonProperty(PropertyName = "node")]
        public string Node { get; set; }
        [JsonProperty(PropertyName = "reference_node")]
        public string ReferenceNode { get; set; }
        [JsonProperty(PropertyName = "transaction")]
        public string Transaction { get; set; }
        [JsonProperty(PropertyName = "neo4j_version")]
        public string Version { get; set; }
    }
}
