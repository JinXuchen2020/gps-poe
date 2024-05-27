using POEMgr.Domain.Cores;
using POEMgr.Domain.IRepositories;
using POEMgr.Repository.UnitOfWork;
using System.Linq.Expressions;

namespace POEMgr.Repository.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>, IPaging<TEntity> where TEntity : AggregateRoot
    {
        internal IEFUnitOfWork unitOfWork { set; get; }

        public Repository(IEFUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IQueryable<TEntity> Entities
        {
            get { return unitOfWork.context.Set<TEntity>(); }
        }

        public virtual TEntity GetById(dynamic id)
        {
            return unitOfWork.context.Set<TEntity>().Find(id);
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            return unitOfWork.context.Set<TEntity>().Where(lamada).Single();
        }

        public virtual TEntity GetFirst(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            return unitOfWork.context.Set<TEntity>().Where(lamada).First();
        }

        public virtual List<TEntity> GetList()
        {
            return unitOfWork.context.Set<TEntity>().AsQueryable<TEntity>().ToList();
        }

        public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            return unitOfWork.context.Set<TEntity>().Where(lamada).AsQueryable<TEntity>().ToList();
        }

        public virtual bool IsAny(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            return unitOfWork.context.Set<TEntity>().Where(lamada).Any();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            return unitOfWork.context.Set<TEntity>().Where(lamada).Count();
        }

        public virtual bool Insert(TEntity entity)
        {
            unitOfWork.RegisterNew(entity);
            return unitOfWork.Commit();
        }

        public virtual bool Insert(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                unitOfWork.RegisterNew(entity);
            }

            return unitOfWork.Commit();
        }

        public virtual bool Delete(object id)
        {
            var entity = unitOfWork.context.Set<TEntity>().Find(id);
            if (entity == null) return false;

            unitOfWork.RegisterDeleted(entity);
            return unitOfWork.Commit();
        }

        public virtual bool Delete(object[] ids)
        {
            var entities = unitOfWork.context.Set<TEntity>().Where(x => ids.Contains(x));

            foreach (var entity in entities)
            {
                unitOfWork.RegisterDeleted(entity);
            }

            return unitOfWork.Commit();
        }

        public virtual bool Delete(TEntity entity)
        {
            unitOfWork.RegisterDeleted(entity);
            return unitOfWork.Commit();
        }

        public virtual bool Delete(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                unitOfWork.RegisterDeleted(entity);
            }

            return unitOfWork.Commit();
        }

        public virtual bool Delete(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            var entities = unitOfWork.context.Set<TEntity>().Where(lamada);

            foreach (var entity in entities)
            {
                unitOfWork.RegisterDeleted(entity);
            }

            return unitOfWork.Commit();
        }

        public virtual bool Update(TEntity entity)
        {
            unitOfWork.RegisterModified(entity);
            return unitOfWork.Commit();
        }

        public virtual bool Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                unitOfWork.RegisterModified(entity);
            }

            return unitOfWork.Commit();
        }

        public virtual async Task<TEntity> GetByIdAsync(dynamic id)
        {
            return await unitOfWork.context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            return unitOfWork.context.Set<TEntity>().Where(lamada).Single();
        }

        public virtual async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            return unitOfWork.context.Set<TEntity>().Where(lamada).First();
        }

        public virtual async Task<List<TEntity>> GetListAsync()
        {
            return unitOfWork.context.Set<TEntity>().AsQueryable<TEntity>().ToList();
        }

        public virtual async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            return unitOfWork.context.Set<TEntity>().Where(lamada).AsQueryable<TEntity>().ToList();
        }

        public virtual async Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            return unitOfWork.context.Set<TEntity>().Where(lamada).Any();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            return unitOfWork.context.Set<TEntity>().Where(lamada).Count();
        }

        public virtual async Task<bool> InsertAsync(TEntity entity)
        {
            unitOfWork.RegisterNew(entity);
            return await unitOfWork.CommitAsync();
        }

        public virtual async Task<bool> InsertAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                unitOfWork.RegisterNew(entity);
            }

            return await unitOfWork.CommitAsync();
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            var entity = unitOfWork.context.Set<TEntity>().Find(id);
            if (entity == null) return false;

            unitOfWork.RegisterDeleted(entity);
            return await unitOfWork.CommitAsync();
        }

        public virtual async Task<bool> DeleteAsync(object[] ids)
        {
            var entities = unitOfWork.context.Set<TEntity>().Where(x => ids.Contains(x));

            foreach (var entity in entities)
            {
                unitOfWork.RegisterDeleted(entity);
            }

            return await unitOfWork.CommitAsync();
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            unitOfWork.RegisterDeleted(entity);
            return await unitOfWork.CommitAsync();
        }

        public virtual async Task<bool> DeleteAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                unitOfWork.RegisterDeleted(entity);
            }

            return await unitOfWork.CommitAsync();
        }

        public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> express)
        {
            Func<TEntity, bool> lamada = express.Compile();
            var entities = unitOfWork.context.Set<TEntity>().Where(lamada);

            foreach (var entity in entities)
            {
                unitOfWork.RegisterDeleted(entity);
            }

            return await unitOfWork.CommitAsync();
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            unitOfWork.RegisterModified(entity);
            return await unitOfWork.CommitAsync();
        }

        public virtual async Task<bool> UpdateAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                unitOfWork.RegisterModified(entity);
            }

            return await unitOfWork.CommitAsync();
        }

        public (int count, List<TEntity> data) GetPageList(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public (int count, List<TEntity> data) GetPageList(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderByExpression = null, Domain.Cores.OrderByType orderByType = Domain.Cores.OrderByType.Asc)
        {
            throw new NotImplementedException();
        }

        public Task<(int count, List<TEntity> data)> GetPageListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<(int count, List<TEntity> data)> GetPageListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderByExpression = null, Domain.Cores.OrderByType orderByType = Domain.Cores.OrderByType.Asc)
        {
            throw new NotImplementedException();
        }
    }
}
