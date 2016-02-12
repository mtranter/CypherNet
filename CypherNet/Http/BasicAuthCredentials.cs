namespace CypherNet.Http
{
    using System;
    using System.Text;

    public class BasicAuthCredentials
    {
        public BasicAuthCredentials(string username, string password)
        {
            EncodedCredentials = EncodeCredentials(username, password);
        }

        public string EncodedCredentials { get; }

        private static string EncodeCredentials(string username, string password)
        {
            var auth = string.Format("{0}:{1}", username, password);
            var enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
            var cred = string.Format("{0} {1}", "Basic", enc);
            return cred;
        }
    }
}