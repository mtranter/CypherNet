namespace CypherNet.Transaction
{
    using System;
    using Http;

    internal class TransactionalCypherClient : IRawCypherClient, ICypherUnitOfWork
    {
        private readonly string _baseUri;
        private readonly IWebClient _webClient;

        internal TransactionalCypherClient(string baseUri, IWebClient webClient)
        {
            _baseUri = baseUri;
            _webClient = webClient;
        }

        public TResult Run<TResult>(string cypher)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public bool KeepAlive()
        {
            throw new NotImplementedException();
        }
    }
}