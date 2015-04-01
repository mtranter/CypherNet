using CypherNet.Serialization;

namespace CypherNet.Transaction
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using Http;

    #endregion

    internal interface ICypherClientFactory
    {
        ICypherClient Create();
    }

    internal class CypherClientFactory : ICypherClientFactory
    {
        private static readonly Dictionary<string, ICypherClient> ActiveClients =
            new Dictionary<string, ICypherClient>();

        private static readonly object Lock = new object();
        private readonly string _baseUri;
        private readonly string _username;
        private readonly string _password;
        private readonly IWebClient _webClient;
        private readonly IWebSerializer _serializer;
        private readonly IEntityCache _entityCache;

        public CypherClientFactory(string baseUri, string username, string password, IWebClient webClient, IWebSerializer serializer, IEntityCache entityCache)
        {
            _baseUri = baseUri;
            _username = username;
            _password = password;
            _webClient = webClient;
            _serializer = serializer;
            _entityCache = entityCache;
        }

        public ICypherClient Create()
        {
            if (Transaction.Current != null)
            {
                lock (Lock)
                {
                    var key = Transaction.Current.TransactionInformation.LocalIdentifier;
                    ICypherClient client;
                    if (ActiveClients.ContainsKey(key))
                    {
                        client = ActiveClients[key];
                    }
                    else
                    {
                        client = new TransactionalCypherClient(_baseUri, _username, _password, _webClient, _serializer, this._entityCache);
                        var notifier = new ResourceManager((ICypherUnitOfWork) client);

                        notifier.Complete += (o, e) =>
                                                 {
                                                     lock (Lock)
                                                     {
                                                         ActiveClients.Remove(key);
                                                     }
                                                 };

                        ActiveClients.Add(key, client);
                        Transaction.Current.EnlistVolatile(notifier, EnlistmentOptions.EnlistDuringPrepareRequired);
                    }
                    return client;
                }
            }

            return new NonTransactionalCypherClient(_baseUri, _username, _password, _webClient, _serializer);
        }

        private class ResourceManager : IEnlistmentNotification
        {
            private readonly ICypherUnitOfWork _unitOfWork;

            internal ResourceManager(ICypherUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            #region IEnlistmentNotification Members

            public void Commit(Enlistment enlistment)
            {
                _unitOfWork.Commit();
                OnComplete();
                enlistment.Done();
            }

            public void InDoubt(Enlistment enlistment)
            {
                enlistment.Done();
            }

            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                if (_unitOfWork.KeepAlive())
                {
                    preparingEnlistment.Prepared();
                }
                else
                {
                    preparingEnlistment.ForceRollback();
                }
            }

            public void Rollback(Enlistment enlistment)
            {
                _unitOfWork.Rollback();
                OnComplete();
            }

            #endregion

            internal event EventHandler Complete;

            private void OnComplete()
            {
                var handler = Complete;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
    }
}