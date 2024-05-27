using POEMgr.Domain.IRepositories;
using POEMgr.Domain.Models;
using POEMgr.Repository.UnitOfWork;

namespace POEMgr.Repository.Repositories
{
    internal class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(IEFUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
