using CypherNet.Queries;

namespace CypherNet.Transaction
{
    using System;
    using Http;

    internal class NonTransactionalCypherClient : IRawCypherClient
    {
        private readonly string _baseUri;
        private readonly IWebClient _webClient;

        internal NonTransactionalCypherClient(string baseUri, IWebClient webClient)
        {
            _baseUri = UriHelper.Combine(baseUri, "transaction/commit");
            _webClient = webClient;
        }

        #region IRawCypherClient Members

        public System.Collections.Generic.IEnumerable<TOut> ExecuteQuery<TOut>(string cypherQuery)
        {
            var request = CypherQueryRequest.Create(cypherQuery);
            var responseTask = _webClient.PostAsync<CypherResponse<TOut>>(_baseUri, request);
            var response = responseTask.Result;

            return response.Results;
        }

        public void ExecuteCommand(string cypherCommand)
        {
            var request = CypherQueryRequest.Create(cypherCommand);
            _webClient.PostAsync<CypherResponse<object>>(_baseUri, request);
        }

        #endregion
    }
}