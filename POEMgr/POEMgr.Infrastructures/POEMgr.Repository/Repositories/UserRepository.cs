using Microsoft.EntityFrameworkCore;
using POEMgr.Domain.IRepositories;
using POEMgr.Domain.Models;
using POEMgr.Repository.UnitOfWork;
using System.Linq.Expressions;

namespace POEMgr.Repository.Repositories
{
    internal class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IEFUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override async Task<User> GetByIdAsync(dynamic id)
        {
            var key = (Guid)id;
            return await unitOfWork.context.Users.Include(_ => _.Roles).FirstOrDefaultAsync(_ => _.Id == key);
        }

        public override List<User> GetList(Expression<Func<User, bool>> express)
        {
            Func<User, bool> lamada = express.Compile();
            return unitOfWork.context.Users.Include(x => x.Roles).Where(lamada).AsQueryable().ToList();
        }
    }
}
