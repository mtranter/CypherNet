﻿using System.Linq;
using CypherNet.Logging;
using CypherNet.Serialization;

namespace CypherNet.Transaction
{
    #region

    using System.Collections.Generic;
    using Http;
    using Queries;

    #endregion

    internal class NonTransactionalCypherClient : ICypherClient
    {
        private readonly string _baseUri;
        private readonly string _username;
        private readonly string _password;
        private readonly IWebClient _webClient;
        private readonly IWebSerializer _serializer;

        internal NonTransactionalCypherClient(string baseUri, string username, string password, IWebClient webClient, IWebSerializer serializer)
        {
            _baseUri = UriHelper.Combine(baseUri, "transaction/commit");
            _username = username;
            _password = password;
            _webClient = webClient;
            _serializer = serializer;
        }

        #region IRawCypherClient Members

        public IEnumerable<TOut> ExecuteQuery<TOut>(string cypherQuery)
        {
            var request = CypherQueryRequest.Create(cypherQuery);
            var srequest = _serializer.Serialize(request);
            Logger.Current.Log("Executing: " + srequest, LogLevel.Info);
            var responseTask = _webClient.PostAsync(_baseUri, _username, _password, srequest);
            responseTask.Wait();
            var readTask = responseTask.Result.Content.ReadAsStringAsync();
            readTask.Wait();
            var response = readTask.Result;
            Logger.Current.Log("Response: " + response, LogLevel.Info);
            var cypherResponse = _serializer.Deserialize<CypherResponse<TOut>>(response);

            if (cypherResponse.Errors.Any())
            {
                throw new CypherResponseException(cypherResponse.Errors.Select(e => e.Message).ToArray());
            }

            return cypherResponse.Results ?? Enumerable.Empty<TOut>();
        }

        public void ExecuteCommand(string cypherCommand)
        {
            ExecuteQuery<dynamic>(cypherCommand);
        }

        #endregion
    }
}