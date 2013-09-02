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
            _baseUri = baseUri;
            _webClient = webClient;
        }

        #region IRawCypherClient Members

        public System.Collections.Generic.IEnumerable<TOut> ExecuteQuery<TOut>(string cypherQuery)
        {
            throw new NotImplementedException();
        }

        public void ExecuteCommand(string cypherCommand)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}