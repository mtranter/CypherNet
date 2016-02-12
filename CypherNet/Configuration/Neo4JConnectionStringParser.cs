using System.Linq;
using System.Text.RegularExpressions;

namespace CypherNet.Configuration
{
    internal class Neo4JConnectionStringParser
    {
        private readonly static Regex URL_REGEX = new Regex(@"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");

        internal static ConnectionProperties Parse(string connectionString)
        {
            if (URL_REGEX.IsMatch(connectionString))
            {
                return new ConnectionProperties(connectionString, null, null);
            }
            var values = connectionString.Split(';')
                .Select(s => s.Trim())
                .Select(s => s.Split('='))
                .ToDictionary(arr => arr[0].ToLower(), arr => arr[1]);

            var server = values["server"];
            string user, password;

            values.TryGetValue("user id", out user);
            values.TryGetValue("password", out password);
            return new ConnectionProperties(server, user, password);
        }
    }
}