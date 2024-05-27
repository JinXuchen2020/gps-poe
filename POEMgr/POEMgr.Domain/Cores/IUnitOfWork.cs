namespace POEMgr.Domain.Cores
{
    public interface IUnitOfWork
    {
        bool IsCommitted { get; set; }
        bool Commit();
        Task<bool> CommitAsync();
        void Rollback();
    }
}
