using System.Linq.Expressions;

namespace POEMgr.Domain.Cores
{
    public interface IPaging<TEntity> where TEntity : Entity
    {
        (int count, List<TEntity> data) GetPageList(
            Expression<Func<TEntity, bool>> whereExpression,
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Asc);

        Task<(int count, List<TEntity> data)> GetPageListAsync(Expression<Func<TEntity, bool>> whereExpression,
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Asc);
    }

    public enum OrderByType
    {
        Asc,
        Desc
    }
}
