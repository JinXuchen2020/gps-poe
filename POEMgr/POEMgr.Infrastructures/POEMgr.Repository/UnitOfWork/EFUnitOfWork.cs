using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using POEMgr.Domain.Cores;
using POEMgr.Repository.DBContext;

namespace POEMgr.Repository.UnitOfWork
{
    public class EFUnitOfWork : IEFUnitOfWork
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        //private Guid CurrentUserId => httpContextAccessor.CurrentUserId();

        public POEContext context { get { return EFContext; } }
        public POEContext EFContext { get; set; }

        public EFUnitOfWork(POEContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.EFContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public void RegisterNew<TEntity>(TEntity obj) where TEntity : AggregateRoot
        {
            var state = context.Entry(obj).State;
            if (state == EntityState.Detached)
            {
                obj.CreatedTime = obj.ModifiedTime = DateTime.Now;
                //obj.CreatedBy = obj.ModifiedBy = CurrentUserId;
                context.Entry(obj).State = EntityState.Added;
            }
            IsCommitted = false;
        }

        public void RegisterModified<TEntity>(TEntity obj) where TEntity : AggregateRoot
        {
            if (context.Entry(obj).State == EntityState.Detached)
            {

                context.Set<TEntity>().Attach(obj);
            }

            obj.ModifiedTime = DateTime.Now;
            //obj.ModifiedBy = CurrentUserId;
            context.Entry(obj).State = EntityState.Modified;
            IsCommitted = false;
        }

        public void RegisterDeleted<TEntity>(TEntity obj) where TEntity : AggregateRoot
        {
            obj.ModifiedTime = DateTime.Now;
            //obj.ModifiedBy = CurrentUserId;
            obj.IsDeleted = true;
            context.Entry(obj).State = EntityState.Modified;
            //context.Entry(obj).State = EntityState.Deleted;
            IsCommitted = false;
        }

        public bool IsCommitted { get; set; }

        public bool Commit()
        {
            if (IsCommitted)
            {
                return false;
            }
            try
            {
                int result = context.SaveChanges();
                IsCommitted = true;
                return result > 0;
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        public async Task<bool> CommitAsync()
        {
            if (IsCommitted)
            {
                return false;
            }
            try
            {
                int result = await context.SaveChangesAsync();
                IsCommitted = true;
                return result > 0;
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        public void Rollback()
        {
            IsCommitted = false;
        }

        public void Dispose()
        {
            if (!IsCommitted)
            {
                CommitAsync().GetAwaiter().GetResult();
            }
            context.Dispose();
        }
    }
}
