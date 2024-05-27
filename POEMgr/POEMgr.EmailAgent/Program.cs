using MailService.Interfaces;
using MailService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using POEMgr.EmailAgent.ProcessingCores;
using POEMgr.EmailAgent.Workers;
using POEMgr.Repository.DBContext;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "USB Storage Disabled Service";
    })
    .ConfigureServices((context, services) =>
    {
        LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services);
        services.AddDbContext<POEContext>(options =>
        {
            options.UseSqlServer(context.Configuration.GetConnectionString("DBServer"));
        }, ServiceLifetime.Singleton);

        services.AddSingleton<IEmailService>(provider =>
        {
            var mail = context.Configuration.GetValue<string>("NotificationEmail:Name");
            var password = context.Configuration.GetValue<string>("NotificationEmail:Password");
            return new EmailServiceCore(mail, password);
        });

        services.AddSingleton<POEEmailProcessingCore>(provider =>
        {
            var config = context.Configuration.GetSection("CommonSettings").Get<CommonSetting>();
            var dbContext = provider.GetService<POEContext>();
            var emailService = provider.GetService<IEmailService>();
            return new POEEmailProcessingCore(config, dbContext, emailService);
        });

        services.AddHostedService<POECheckStatusWorker>();
        services.AddHostedService<POESendHttpWorker>();
        services.AddHostedService<POESendTestMailWorker>();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.AddConfiguration(context.Configuration.GetSection("Logging"));
    })
    .Build();

await host.RunAsync();
