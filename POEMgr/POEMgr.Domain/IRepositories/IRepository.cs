using POEMgr.Domain.Cores;
using System.Linq.Expressions;

namespace POEMgr.Domain.IRepositories
{
    public interface IRepository<TEntity> : IPaging<TEntity> where TEntity : Entity
    {
        TEntity GetById(dynamic id);
        TEntity GetSingle(Expression<Func<TEntity, bool>> express);
        TEntity GetFirst(Expression<Func<TEntity, bool>> express);
        List<TEntity> GetList();
        List<TEntity> GetList(Expression<Func<TEntity, bool>> express);
        bool IsAny(Expression<Func<TEntity, bool>> express);
        int Count(Expression<Func<TEntity, bool>> express);

        bool Insert(TEntity entity);
        bool Insert(IEnumerable<TEntity> entities);
        bool Delete(object id);
        bool Delete(object[] ids);
        bool Delete(TEntity entity);
        bool Delete(IEnumerable<TEntity> entities);
        bool Delete(Expression<Func<TEntity, bool>> express);
        bool Update(TEntity entity);
        bool Update(IEnumerable<TEntity> entities);

        Task<TEntity> GetByIdAsync(dynamic id);
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> express);
        Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> express);
        Task<List<TEntity>> GetListAsync();
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> express);
        Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> express);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> express);

        Task<bool> InsertAsync(TEntity entity);
        Task<bool> InsertAsync(IEnumerable<TEntity> entities);
        Task<bool> DeleteAsync(object id);
        Task<bool> DeleteAsync(object[] ids);
        Task<bool> DeleteAsync(TEntity entity);
        Task<bool> DeleteAsync(IEnumerable<TEntity> entities);
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> express);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> UpdateAsync(IEnumerable<TEntity> entities);
    }
}
