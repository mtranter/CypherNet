namespace CypherNet.Transaction
{
    using System;
    using System.Linq;
    using System.Text;
    using Http;
    using Queries;

    internal class TransactionalCypherClient : IRawCypherClient, ICypherUnitOfWork
    {
        private bool _isInitialized;
        private string _transactionUri;
        private readonly IWebClient _webClient;

        internal TransactionalCypherClient(string baseUri, IWebClient webClient)
        {
            _transactionUri = UriHelper.Combine(baseUri, "transaction/");
            _webClient = webClient;
        }

        #region IRawCypherClient Members

        public System.Collections.Generic.IEnumerable<TOut> ExecuteQuery<TOut>(string cypherQuery)
        {
            var request = CypherQueryRequest.Create(cypherQuery);
            var responseTask = _webClient.PostAsync<CypherResponse<TOut>>(_transactionUri, request);
            var response = responseTask.Result;
            if (!_isInitialized)
            {
                _transactionUri = response.Commit;
                _isInitialized = true;
            }
            return response.Results;
        }

        public void ExecuteCommand(string cypherCommand)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Commit()
        {
            var commitUri = UriHelper.Combine(_transactionUri, "commit");
            var emptyRequest = new CypherQueryRequest();
            var resultTask = _webClient.PostAsync<object>(commitUri, emptyRequest);
            var result = resultTask.Result;

        }

        public void Rollback()
        {
            var commitUri = UriHelper.Combine(_transactionUri, "rollback");
            var emptyRequest = new CypherQueryRequest();
            var resultTask = _webClient.PostAsync<CypherResponse<object>>(commitUri, emptyRequest);
            var result = resultTask.Result;
            if (result.Errors.Any())
                throw new Exception("Errors returned from Neo Server: " + String.Join(",", result.Errors));
        }

        public bool KeepAlive()
        {
            var emptyRequest = new CypherQueryRequest();
            var resultTask = _webClient.PostAsync<CypherResponse<object>>(_transactionUri, emptyRequest);
            var result = resultTask.Result;
            if (result.Errors.Any())
                throw new Exception("Errors returned from Neo Server: " + String.Join(",", result.Errors));
            return true;
        }

    }
}