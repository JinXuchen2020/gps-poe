using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using POEMgr.Domain.IRepositories;
using POEMgr.Repository.DBContext;
using POEMgr.Repository.Repositories;
using POEMgr.Repository.UnitOfWork;

namespace POEMgr.Repository
{
    public static class RepositoryServiceInject
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<POEContext>(options => { options.UseSqlServer(connectionString); }, ServiceLifetime.Scoped);
            //services.AddScoped<IEFUnitOfWork, EFUnitOfWork>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IRoleRepository, RoleRepository>();
            return services;
        }

    }
}
