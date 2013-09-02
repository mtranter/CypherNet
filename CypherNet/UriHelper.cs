namespace CypherNet.Transaction
{
    public class UriHelper
    {

        public static string Combine(params string[] values)
        {
            var retval = values[0];
            for (var i = 1; i < values.Length; i++)
            {
                var delim = values[i - 1].EndsWith("/") ? "" : "/";
                var current = values[i].StartsWith("/") ? values[i].Substring(1, values[i].Length - 1) : values[i];
                retval = retval + delim + current;
            }

            return retval;
        }
    }
}