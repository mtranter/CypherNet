using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CypherNet.Http
{
    #region

    using System.Threading.Tasks;

    #endregion

    public interface IWebClient
    {
        Task<IHttpResponseMessage> GetAsync(string url);

        Task<IHttpResponseMessage> PostAsync(string url, String body);

        Task<IHttpResponseMessage> PutAsync(string url, String body);

        Task<IHttpResponseMessage> DeleteAsync(string url);
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