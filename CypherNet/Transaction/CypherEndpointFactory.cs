

namespace CypherNet.Transaction
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using Http;
    using Serialization;

    public class CypherEndpointFactory
    {
        private static readonly Dictionary<string, ResourceManager> CurrentNotifications =
            new Dictionary<string, ResourceManager>();

        private static readonly object Lock = new object();
        
        public ICypherEndpoint Create(string sourceUri)
        {
            return Create(sourceUri, new WebClient(new DefaultJsonSerializer()));
        }

        public ICypherEndpoint Create(string sourceUri, IWebClient webClient)
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
                    return new CypherEndpoint(unitOfWork);
                }
            }
            else
            {
                return new CypherEndpoint(new NonTransactionalCypherClient(sourceUri, webClient));
            }
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