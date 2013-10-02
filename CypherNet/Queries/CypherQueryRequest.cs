namespace CypherNet.Queries
{
    #region

    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;

    #endregion

    internal class CypherQueryRequest
    {
        private const string JsonRegex =
            @"{(\s*""\w+""\s*:\s*("".+"")?(\d+[^,])?\s*)(\s*,\s*(\s*""\w+""\s*:\s*("".+"")?(\d+[^,])?\s*))*\}";

        private readonly List<CypherQueryStatement> _statements = new List<CypherQueryStatement>();

        [JsonProperty(PropertyName = "statements")]
        internal IEnumerable<CypherQueryStatement> Statements
        {
            get { return _statements; }
        }

        public static CypherQueryRequest Create(string statement)
        {
            var match = Regex.Match(statement, JsonRegex);
            var request = new CypherQueryRequest();
            var @params = new List<KeyValuePair<string, string>>();
            var count = 0;

            while (match != null && match.Success)
            {
                var paramName = "param_" + count;
                statement = statement.Replace(match.Value, "{" + paramName + "}");
                @params.Add(new KeyValuePair<string, string>(paramName, match.Value));
                count++;
                match = match.NextMatch();
            }
            request.AddStatement(statement, @params.ToArray());
            return request;
        }

        internal void AddStatement(string statement, params KeyValuePair<string, string>[] namedParameters)
        {
            _statements.Add(new CypherQueryStatement(statement, namedParameters));
        }

        internal static CypherQueryRequest Empty()
        {
            return new CypherQueryRequest();
        }
    }
}