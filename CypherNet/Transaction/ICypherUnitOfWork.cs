using System.Net.Http;

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
}
