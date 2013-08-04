using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherNet.UnitTests
{
    using System.Transactions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Transaction;

    [TestClass]
    public class ResourceManagerTests
    {
        [TestMethod]
        public void ResourceManger_UseNoTransactionScope_CommitsOnDisposal()
        {
            var uow = new Mock<ICypherUnitOfWork>();
            using (var rm = new CypherEndpointFactory(uow.Object))
            {

            }
            uow.Verify(u => u.Commit());
        }


        [TestMethod]
        public void ResourceManger_UseTransactionScope_CallsKeepaliveOnPrepare()
        {
            var uow = new Mock<ICypherUnitOfWork>();
            uow.Setup(u => u.KeepAlive()).Returns(true);
            using (var ts = new TransactionScope())
            {
                var rm = new CypherEndpointFactory(uow.Object);
                ts.Complete();
            }
            uow.Verify(u => u.KeepAlive());
        }

        [TestMethod]
        public void ResourceManger_UseTransactionScope_CommitsOnTransactionCommit()
        {
            var uow = new Mock<ICypherUnitOfWork>();
            uow.Setup(u => u.KeepAlive()).Returns(true);
            using (var ts = new TransactionScope())
            {
                var rm = new CypherEndpointFactory(uow.Object);
                ts.Complete();
            }
            uow.Verify(u => u.Commit());
        }


        [TestMethod]
        public void ResourceManger_UseTransactionScope_RollsbackIfTransactionscopeNotCompleted()
        {
            var uow = new Mock<ICypherUnitOfWork>();
            uow.Setup(u => u.KeepAlive()).Returns(true);
            using (var ts = new TransactionScope())
            using (var rm = new CypherEndpointFactory(uow.Object))
            {

            }
            uow.Verify(u => u.Rollback());
        }

        [TestMethod]
        public void ResourceManger_UseNestedTransactionScopes_CommitsInnerScopeButNotOuter()
        {
            var outerUnitOfWork = new Mock<ICypherUnitOfWork>();
            outerUnitOfWork.Setup(u => u.KeepAlive()).Returns(true);
            outerUnitOfWork.Setup(u => u.Commit()).Throws(new Exception("Commit should not be called on outerUnitOfWork"));
            var innerUnitOfWork = new Mock<ICypherUnitOfWork>();
            innerUnitOfWork.Setup(u => u.KeepAlive()).Returns(true);
            using (var outerTs = new TransactionScope())
            {
                var rm = new CypherEndpointFactory(outerUnitOfWork.Object);

                using (var innerTs = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    var rmInner = new CypherEndpointFactory(innerUnitOfWork.Object);
                    innerTs.Complete();
                }
                innerUnitOfWork.Verify(u => u.Commit());
            }
            outerUnitOfWork.Verify(u => u.Rollback());
        }


        [TestMethod]
        public void ResourceManger_UseNestedTransactionScopes_RollsbackInnerScopeButNotOuter()
        {
            var outerUnitOfWork = new Mock<ICypherUnitOfWork>();
            outerUnitOfWork.Setup(u => u.KeepAlive()).Returns(true);
            outerUnitOfWork.Setup(u => u.Rollback()).Throws(new Exception("Rollback should not be called on outerUnitOfWork"));
            var innerUnitOfWork = new Mock<ICypherUnitOfWork>();
            innerUnitOfWork.Setup(u => u.KeepAlive()).Returns(true);
            innerUnitOfWork.Setup(u => u.Commit()).Throws(new Exception("Commit should not be called on innerUnitOfWork"));
            using (var outerTs = new TransactionScope())
            {
                var rm = new CypherEndpointFactory(outerUnitOfWork.Object);

                using (var innerTs = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    var rmInner = new CypherEndpointFactory(innerUnitOfWork.Object);
                }
                innerUnitOfWork.Verify(u => u.Rollback());

                outerTs.Complete();
            }
            outerUnitOfWork.Verify(u => u.Commit());
        }
    }
}

