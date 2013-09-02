namespace CypherNet.Transaction
{
    using System;
    using System.Linq;
    using System.Text;
    using Http;
    using Queries;

    internal class TransactionalCypherClient : IRawCypherClient, ICypherUnitOfWork
    {
        private readonly string _transactionUri;
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
            throw new NotImplementedException();
        }

        public void ExecuteCommand(string cypherCommand)
        {
            throw new NotImplementedException();
        }

        #endregion

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