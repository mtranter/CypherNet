using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using CypherNet.Http;

namespace CypherNet.UnitTests
{
    internal class MockHttpResponseMessage : IHttpResponseMessage
    {

        public MockHttpResponseMessage(string response, HttpStatusCode statusCode,
                                       params KeyValuePair<string, IEnumerable<string>>[] headers)
        {
            StatusCode = statusCode;
            Content = new StringContent(response);
            Headers = new List<KeyValuePair<string, IEnumerable<string>>>(headers);
            IsSuccessStatusCode = (int) StatusCode >= 200 && (int) StatusCode < 300;
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; private set; }
        public HttpContent Content { get; private set; }
        public bool IsSuccessStatusCode { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string ReasonPhrase { get; private set; }
    }
}