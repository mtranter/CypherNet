

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

        private static readonly Dictionary<string, ResourceManager> CurrentNotifications =
            new Dictionary<string, ResourceManager>();

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
                var unitOfWork = new TransactionalCypherClient(sourceUri, webClient);
                var key = Transaction.Current.TransactionInformation.LocalIdentifier;
                var notifier = new ResourceManager(unitOfWork);
                lock (Lock)
                {
                    notifier.Complete += (o, e) =>
                        {
                            lock (Lock)
                            {
                                CurrentNotifications.Remove(key);
                            }
                        };

                    CurrentNotifications.Add(key, notifier);
                    System.Transactions.Transaction.Current.EnlistVolatile(notifier, EnlistmentOptions.EnlistDuringPrepareRequired);
                    return unitOfWork;
                }
            }
            else
            {
                return new NonTransactionalCypherClient(sourceUri, webClient);
            }
        }

        class ResourceManager : IEnlistmentNotification
        {
            private readonly ICypherUnitOfWork _unitOfWork;

            internal ResourceManager(ICypherUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
                ;
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