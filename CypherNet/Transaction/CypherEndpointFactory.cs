using System;
using System.Collections.Generic;
using System.Transactions;

namespace CypherNet.Transaction
{
    class CypherEndpointFactory : IDisposable
    {
        private static readonly Dictionary<string, ResourceManager> CurrentNotifications =
            new Dictionary<string, ResourceManager>();

        private static readonly object Lock = new object();
        private readonly ICypherUnitOfWork _unitOfWork;

        internal CypherEndpointFactory(ICypherUnitOfWork unitOfWork)
        {
            if (System.Transactions.Transaction.Current != null)
            {
                var key = System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier;
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
                }
            }
            else
            {
                _unitOfWork = unitOfWork;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_unitOfWork != null)
            {
                _unitOfWork.Commit();
            }
        }

        #endregion

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

            internal void OnComplete()
            {
                EventHandler handler = Complete;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
    }
}