namespace CypherNet.Queries
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;

    internal class CypherQueryRequest
    {

        const string JsonRegex = @"\{(\s*""\w+""\s*:\s*""?\w+""?\s*)(\s*,\s*(\s*""\w+""\s*:\s*""?\w+""?\s*))*\}";
        public static CypherQueryRequest Create(string statement)
        {
            var matches = Regex.Match(JsonRegex, statement);
            var request = new CypherQueryRequest();
            var @params = new List<CypherQueryParameter>();
            var count = 0;
            foreach (Capture match in matches.Captures)
            {
                var paramName = "Param_" + count;
                statement = statement.Replace(match.Value, paramName);
                @params.Add(new CypherQueryParameter(paramName, match.Value));
                count++;
            }
            request.AddStatement(statement, @params.ToArray());
            return request;
        }

        private readonly List<CypherQueryStatement> _statements = new List<CypherQueryStatement>();

        internal void AddStatement(string statement, params CypherQueryParameter[] parameters)
        {
            _statements.Add(new CypherQueryStatement(statement, parameters));
        }

        [JsonProperty(PropertyName="statements")]
        internal IEnumerable<CypherQueryStatement> Statements
        {
            get { return _statements; }
        }
    }
}