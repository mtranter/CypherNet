namespace CypherNet.Serialization
{
    #region

    using System.Linq;
    using Newtonsoft.Json.Linq;

    #endregion

    internal static class JTokenExtensions
    {
        public static bool HasProperty<TToken>(this TToken token, string property)
            where TToken : JToken
        {
            return token.Children().OfType<JProperty>().Any(j => j.Name == property);
        }
    }
}