using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherNet.Serialization
{
    using Newtonsoft.Json.Linq;

    static class JTokenExtensions
    {
        public static bool HasProperty<TToken>(this TToken token, string property)
            where TToken : JToken
        {
            return token.Children().OfType<JProperty>().Any(j => j.Name == property);
        }
    }
}
