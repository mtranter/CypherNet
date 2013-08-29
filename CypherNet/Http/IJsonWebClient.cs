
namespace CypherNet.Http
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Serialization;

    public interface IWebClient
    {
        Task<TResult> GetAsync<TResult>(string url);

        Task<TResult> PostAsync<TResult>(string url, object body);

        Task<TResult> PutAsync<TResult>(string url, object body);

        Task<TResult> DeleteAsync<TResult>(string url);
    }

    public class WebClient : IWebClient
    {
        private readonly IWebSerializer _serializer;

        public WebClient(IWebSerializer serializer)
        {
            _serializer = serializer;
        }

        #region IWebClient Members

        public async Task<TResult> GetAsync<TResult>(string url)
        {
            return await Execute<TResult>(url, HttpMethod.Get);
        }

        public async Task<TResult> PostAsync<TResult>(string url, object body)
        {
            return await Execute<TResult>(url, body, HttpMethod.Post);
        }

        public async Task<TResult> PutAsync<TResult>(string url, object body)
        {
            return await Execute<TResult>(url, body, HttpMethod.Put);
        }

        public async Task<TResult> DeleteAsync<TResult>(string url)
        {
            return await Execute<TResult>(url, HttpMethod.Delete);
        }

        #endregion


        private async Task<TResult> Execute<TResult>(string url, HttpMethod method)
        {
            var msg = new HttpRequestMessage(method, url);
            using (var client = new HttpClient())
            {
                var result = await client.SendAsync(msg);
                var json = result.Content.ReadAsStringAsync();
                return _serializer.Deserialize<TResult>(json.Result);
            }
        }

        private async Task<TResult> Execute<TResult>(string url, object body, HttpMethod method)
        {
            var msg = new HttpRequestMessage(method, url);
            if (body != null)
            {
                var jsonBody = _serializer.Serialize(body);
                msg.Content = new StringContent(jsonBody);
            }

            using (var client = new HttpClient())
            {
                var result = await client.SendAsync(msg);
                var json = result.Content.ReadAsStringAsync();
                return _serializer.Deserialize<TResult>(json.Result);
            }
        }
    }
}
