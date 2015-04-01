using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CypherNet.Http
{
    #region

    

    #endregion

    public interface IWebClient
    {
        Task<IHttpResponseMessage> PostAsync(string url, string username, string password, string body);

        Task<IHttpResponseMessage> PutAsync(string url, string body, string username, string password);

        Task<IHttpResponseMessage> DeleteAsync(string url, string username, string password);

        Task<IHttpResponseMessage> GetAsync(string url, string username, string password);
    }

    public interface IHttpResponseMessage
    {
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>  Headers { get; }
        HttpContent Content { get; }
        bool IsSuccessStatusCode { get; }
        HttpStatusCode StatusCode { get; }
        string ReasonPhrase { get; }
    }

    class HttpResponseMessageWrapper : IHttpResponseMessage
    {
        private readonly HttpResponseMessage _wrapped;

        public HttpResponseMessageWrapper(HttpResponseMessage wrapped)
        {
            _wrapped = wrapped;
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers
        {
            get { return _wrapped.Headers; }
        }

        public HttpContent Content
        {
            get { return _wrapped.Content; }
        }

        public bool IsSuccessStatusCode
        {
            get { return _wrapped.IsSuccessStatusCode; }
        }

        public HttpStatusCode StatusCode
        {
            get { return _wrapped.StatusCode; }
        }

        public string ReasonPhrase
        {
            get { return _wrapped.ReasonPhrase; }
        }
    }
}