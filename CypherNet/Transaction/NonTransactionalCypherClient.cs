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

        public TResult Run<TResult>(string cypher)
        {
            throw new NotImplementedException();
        }
    }
}