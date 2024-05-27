using POEMgr.Domain.Cores;
using POEMgr.Repository.DBContext;

namespace POEMgr.Repository.UnitOfWork
{
    public interface IEFUnitOfWork : IUnitOfWork, IDisposable
    {
        POEContext context { get; }

        void RegisterNew<TEntity>(TEntity obj) where TEntity : AggregateRoot;
        void RegisterModified<TEntity>(TEntity obj) where TEntity : AggregateRoot;
        void RegisterDeleted<TEntity>(TEntity obj) where TEntity : AggregateRoot;
    }
}
