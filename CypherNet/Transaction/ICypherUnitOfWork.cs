

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Transactions;
using CypherNet.Queries;

namespace CypherNet.Transaction
{
    public interface ICypherUnitOfWork
    {
        void Commit();
        void Rollback();
        bool KeepAlive();
    }

    public enum EndpointState
    {
        Uninitialised,
        Active,
        Complete
    }

    class TransactionalEndpoint : ITransactionalEndpoint
    {
        private static readonly object Lock = new object();
        private static readonly Dictionary<string, ResourceManager> CurrentNotifications = new Dictionary<string, ResourceManager>();

        private readonly IRawCypherClient _requestBuilder;

        private TransactionalEndpoint(IRawCypherClient requestBuilder)
        {
            _requestBuilder = requestBuilder;
        }

        public EndpointState State { get; private set; }

        public ICypherQueryStart<TVariables> BeginQuery<TVariables>()
        {
            throw new System.NotImplementedException();
        }

        public ICypherQueryStart<TVariables> BeginQuery<TVariables>(System.Linq.Expressions.Expression<System.Func<TVariables>> variablePrototype)
        {
            throw new System.NotImplementedException();
        }

        public ICypherQueryReturnOnly<Graph.Node> CreateNode(object properties)
        {
            throw new System.NotImplementedException();
        }

        public ICypherQueryReturnOnly<Graph.Node> CreateNode(object properties, string label)
        {
            throw new System.NotImplementedException();
        }

        void ICypherUnitOfWork.Commit()
        {
            throw new System.NotImplementedException();
        }

        void ICypherUnitOfWork.Rollback()
        {
            throw new System.NotImplementedException();
        }

        bool ICypherUnitOfWork.KeepAlive()
        {
            throw new System.NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            if (this.State == EndpointState.Active)
            {
                ((ICypherUnitOfWork) this).Rollback();
            }
        }

        class ResourceManager : IEnlistmentNotification
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

    internal interface IRawCypherClient : ICypherUnitOfWork
    {
        TResult BuildCypherRequest<TResult>(string cypher);
    }
}
