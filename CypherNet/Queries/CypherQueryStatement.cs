namespace CypherNet.Queries
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class CypherQueryStatement
    {
        public CypherQueryStatement(string statement, IEnumerable<CypherQueryParameter> parameters)
        {
            Statement = statement;
            Parameters = parameters;
        }

        [JsonProperty(PropertyName = "statement")]
        public string Statement { get; private set; }

        [JsonProperty(PropertyName = "parameters")]
        public IEnumerable<CypherQueryParameter> Parameters { get; private set; }
    }
}