using FileService;
using LogService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using POEMgr.Application.Interfaces;
using POEMgr.Application.Services;
using POEMgr.Repository.DBContext;

namespace POEMgr.Application
{
    public static class ServiceInject
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPoeLogService, PoeLogService>();
            services.AddScoped<IPoeFileService, PoeFileService>();
            services.AddScoped<IPoeEmailService, PoeEmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPoeRequestService, PoeRequestService>();
            services.AddScoped<IIncentiveService, IncentiveService>();
            return services;
        }
    }
}
