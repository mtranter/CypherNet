using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CypherNet.Http
{
    public class WebClient : IWebClient
    {
        private readonly BasicAuthCredentials _credentials;

        public WebClient()
        {
        }

        public WebClient(BasicAuthCredentials credentials)
        {
            _credentials = credentials;
        }

        #region IWebClient Members

        public async Task<IHttpResponseMessage> GetAsync(string url)
        {
            return await Execute(url, HttpMethod.Get).ConfigureAwait(false);
        }

        public async Task<IHttpResponseMessage> PostAsync(string url, String body)
        {
            return await Execute(url, body, HttpMethod.Post).ConfigureAwait(false);
        }

        public async Task<IHttpResponseMessage> PutAsync(string url, String body)
        {
            return await Execute(url, body, HttpMethod.Put).ConfigureAwait(false);
        }

        public async Task<IHttpResponseMessage> DeleteAsync(string url)
        {
            return await Execute(url, HttpMethod.Delete).ConfigureAwait(false);
        }

        #endregion

        private Task<IHttpResponseMessage> Execute(string url, HttpMethod method)
        {
            return Execute(url, null, method);
        }

        private async Task<IHttpResponseMessage> Execute(string url, String content, HttpMethod method)
        {
            var msg = new HttpRequestMessage(method, url);

            if (content != null)
            {
                msg.Content = new StringContent(content);
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            using (var client = new HttpClient())
            {
                if (_credentials != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials.EncodedCredentials);
                }

                var result = await client.SendAsync(msg).ConfigureAwait(false);
                return new HttpResponseMessageWrapper(result);
            }
        }
    }
}