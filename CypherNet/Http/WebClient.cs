using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CypherNet.Http
{
    public class WebClient : IWebClient
    {
        #region IWebClient Members
       
        public async Task<IHttpResponseMessage> PostAsync(string url, string username, string password, string body)
        {
            return await Execute(url, username, password, body, HttpMethod.Post).ConfigureAwait(false);
        }

        public async Task<IHttpResponseMessage> PutAsync(string url, string body, string username, string password)
        {
            return await Execute(url, username, password, body, HttpMethod.Put).ConfigureAwait(false);
        }

        public async Task<IHttpResponseMessage> DeleteAsync(string url, string username, string password)
        {
            return await Execute(url, username, password, HttpMethod.Delete).ConfigureAwait(false);
        }

        public async Task<IHttpResponseMessage> GetAsync(string url, string username, string password)
        {
            return await Execute(url, username, password, HttpMethod.Get).ConfigureAwait(false);
        }

        #endregion
        private async Task<IHttpResponseMessage> Execute(string url, string username, string password, HttpMethod method)
        {
            var msg = new HttpRequestMessage(method, url);
            var cred = EncodeCredentials(username, password);
                       
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cred);
                var result = await client.SendAsync(msg).ConfigureAwait(false);
                return new HttpResponseMessageWrapper(result);
            }
        }

        private static string EncodeCredentials(string username, string password)
        {
            string auth = string.Format("{0}:{1}", username, password);
            string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
            string cred = string.Format("{0} {1}", "Basic", enc);
            return cred;
        }



        private async Task<IHttpResponseMessage> Execute(string url, string username, string password, String content, HttpMethod method)
        {
            var msg = new HttpRequestMessage(method, url);
            var cred = EncodeCredentials(username, password);

            if (content != null)
            {
                msg.Content = new StringContent(content);
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cred);
                var result = await client.SendAsync(msg).ConfigureAwait(false);
                return new HttpResponseMessageWrapper(result);
            }
        }
    }

}