namespace CypherNet.Transaction
{
    public interface ICypherUnitOfWork
    {
        void Commit();
        void Rollback();
        bool KeepAlive();
    }
}