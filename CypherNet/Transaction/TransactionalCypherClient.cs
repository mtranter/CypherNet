using CypherNet.Logging;
using CypherNet.Serialization;

namespace CypherNet.Transaction
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Http;
    using Queries;

    #endregion

    internal class TransactionalCypherClient : ICypherClient, ICypherUnitOfWork
    {
        private readonly IWebClient _webClient;
        private readonly IWebSerializer _serializer;
        private bool _isInitialized;
        private string _transactionUri;

        internal TransactionalCypherClient(string baseUri, IWebClient webClient, IWebSerializer serializer)
        {
            _transactionUri = UriHelper.Combine(baseUri, "transaction/");
            _webClient = webClient;
            _serializer = serializer;
        }

        #region IRawCypherClient Members

        public IEnumerable<TOut> ExecuteQuery<TOut>(string cypherQuery)
        {
            var request = CypherQueryRequest.Create(cypherQuery);
            var srequest = _serializer.Serialize(request);
            Logger.Current.Log("Executing: " + srequest, LogLevel.Info);
            var responseTask = _webClient.PostAsync(_transactionUri, srequest);
            var response = responseTask.Result.Content.ReadAsStringAsync().Result;
            Logger.Current.Log("Response: " + response, LogLevel.Info);
            var cypherResponse = _serializer.Deserialize<CypherResponse<TOut>>(response);
            if (cypherResponse.Errors.Any())
            {
                throw new CypherResponseException(cypherResponse.Errors);
            }
            if (!_isInitialized)
            {
                _transactionUri = cypherResponse.Commit.Substring(0, cypherResponse.Commit.Length - ("/commit").Length);
                _isInitialized = true;
            }
            return cypherResponse.Results;

        }

        public void ExecuteCommand(string cypherCommand)
        {
            ExecuteQuery<dynamic>(cypherCommand);
        }

        #endregion

        public void Commit()
        {
            var commitUri = UriHelper.Combine(_transactionUri, "commit");
            var emptyRequest = CypherQueryRequest.Empty();
            Logger.Current.Log("Executing: " + emptyRequest, LogLevel.Info);
            var srequest = _serializer.Serialize(emptyRequest);
            var resultTask = _webClient.PostAsync(commitUri, srequest);
            var response = resultTask.Result.Content.ReadAsStringAsync().Result;
            Logger.Current.Log("Response: " + response, LogLevel.Info);
            var cypherResponse = _serializer.Deserialize<CypherResponse<dynamic>>(response);
            if (cypherResponse.Errors.Any())
            {
                throw new Exception("Errors returned from Neo Server: " + String.Join(",", cypherResponse.Errors));
            }
        }

        public void Rollback()
        {
            var resultTask = _webClient.DeleteAsync(_transactionUri);
            var response = resultTask.Result.Content.ReadAsStringAsync().Result;
            var cypherResponse = _serializer.Deserialize<CypherResponse<dynamic>>(response);
            if (cypherResponse.Errors.Any())
            {
                throw new Exception("Errors returned from Neo Server: " + String.Join(",", cypherResponse.Errors));
            }
        }

        public bool KeepAlive()
        {
            var emptyRequest = CypherQueryRequest.Empty();
            var srequest = _serializer.Serialize(emptyRequest);
            Logger.Current.Log("Executing: " + srequest, LogLevel.Info);
            var resultTask = _webClient.PostAsync(_transactionUri, srequest);
            var response = resultTask.Result.Content.ReadAsStringAsync().Result;
            Logger.Current.Log("Response: " + response, LogLevel.Info);
            var cypherResponse = _serializer.Deserialize<CypherResponse<dynamic>>(response);
            if (cypherResponse.Errors.Any())
            {
                throw new Exception("Errors returned from Neo Server: " + String.Join(",", cypherResponse.Errors));
            }
            return true;
        }
    }
}