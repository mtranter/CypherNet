

namespace CypherNet.Transaction
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using Http;
    using Serialization;

    public interface ICypherClientFactory
    {
        ICypherClient Create();
        ICypherClient Create(string sourceUri);
        ICypherClient Create(string sourceUri, IWebSerializer webSerializer);
        ICypherClient Create(string sourceUri, IWebClient webClient);
    }

    internal class CypherClientFactory : ICypherClientFactory
    {
        private readonly string _baseUri;

        private static readonly Dictionary<string, ICypherClient> ActiveClients =
            new Dictionary<string, ICypherClient>();

        private static readonly object Lock = new object();

        public CypherClientFactory(string baseUri)
        {
            _baseUri = baseUri;
        }

        public ICypherClient Create()
        {
            return Create(_baseUri, new DefaultJsonSerializer());
        }
        
        public ICypherClient Create(string sourceUri)
        {
            return Create(sourceUri, new DefaultJsonSerializer());
        }

        public ICypherClient Create(string sourceUri, IWebSerializer webSerializer)
        {
            return Create(sourceUri, new WebClient(webSerializer));
        }

        public ICypherClient Create(string sourceUri, IWebClient webClient)
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
                        client = new TransactionalCypherClient(sourceUri, webClient);
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

            return new NonTransactionalCypherClient(sourceUri, webClient);
        }

        class ResourceManager : IEnlistmentNotification
        {

            internal ResourceManager(ICypherUnitOfWork unitOfWork)
            {
                UnitOfWork = unitOfWork;
                
            }

            internal ICypherUnitOfWork UnitOfWork { get; private set; }

            #region IEnlistmentNotification Members

            public void Commit(Enlistment enlistment)
            {
                UnitOfWork.Commit();
                OnComplete();
                enlistment.Done();
            }

            public void InDoubt(Enlistment enlistment)
            {
                enlistment.Done();
            }

            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                if (UnitOfWork.KeepAlive())
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
                UnitOfWork.Rollback();
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