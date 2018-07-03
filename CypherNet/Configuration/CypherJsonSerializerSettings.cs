using System;
using Newtonsoft.Json;

namespace CypherNet.Configuration
{
    public static class CypherJsonSerializerSettings
    {
        public static Func<JsonSerializerSettings> DefaultSerializerSettings { get; set; }

        static CypherJsonSerializerSettings()
        {
            DefaultSerializerSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}
