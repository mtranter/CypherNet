namespace CypherNet.Queries
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    #endregion

    internal class CypherQueryStatement
    {
        public CypherQueryStatement(string statement, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            Statement = statement;
            Parameters = parameters ?? Enumerable.Empty<KeyValuePair<string, string>>();
        }

        [JsonProperty(PropertyName = "statement")]
        public string Statement { get; private set; }

        [JsonIgnore]
        public IEnumerable<KeyValuePair<string, string>> Parameters { get; private set; }

        [JsonProperty(PropertyName = "parameters")]
        private IDictionary<string, JObject> ParametersDictionary
        {
            get { return Parameters.ToDictionary(k => k.Key, v => JObject.Parse(v.Value)); }
        }
    }
}