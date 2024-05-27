using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;

namespace POEMgr.ScheduleJob
{
    public static class QuartzService
    {
        private static IConfiguration _configuration;
        public static void StartJob<TJob>() where TJob : IJob
        {
            _configuration = CZHttpContext.ServiceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            var scheduler = new StdSchedulerFactory().GetScheduler().Result;

            var job = JobBuilder.Create<TJob>()
              .WithIdentity("job")
              .Build();

            var StartCheck = TriggerBuilder.Create()
              .WithIdentity("job.trigger")
              .StartNow()
              .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(Convert.ToDouble(_configuration["JobWithInterval:StartCheck"]))).RepeatForever())
              .ForJob(job)
              .Build();

            scheduler.AddJob(job, true);
            scheduler.ScheduleJob(job, StartCheck);
            scheduler.Start();
        }

        public static void StartJobs<TJob>() where TJob : IJob
        {
            _configuration = CZHttpContext.ServiceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            var scheduler = new StdSchedulerFactory().GetScheduler().Result;

            var job = JobBuilder.Create<TJob>()
              .WithIdentity("jobs")
              .Build();

            var StartCheck = TriggerBuilder.Create()
              .WithIdentity("job.StartCheck")
              .StartNow()
              .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(Convert.ToDouble(_configuration["JobWithInterval:StartCheck"]))).RepeatForever())
              .ForJob(job)
              .Build();

            var SendHttpRequest = TriggerBuilder.Create()
              .WithIdentity("job.SendHttpRequest")
              .StartNow()
              .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(Convert.ToDouble(_configuration["JobWithInterval:SendHttpRequest"]))).RepeatForever())
              .ForJob(job)
              .Build();

            var SendMailTest = TriggerBuilder.Create()
              .WithIdentity("job.SendMailTest")
              .StartNow()
              .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(Convert.ToDouble(_configuration["JobWithInterval:SendMailTest"]))).RepeatForever())
              .ForJob(job)
              .Build();

            var dictionary = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>
            {
                {job, new HashSet<ITrigger> {StartCheck, SendHttpRequest, SendMailTest}}
            };
            scheduler.ScheduleJobs(dictionary, true);
            scheduler.Start();
        }

        public static void AddQuartz(this IServiceCollection services, params Type[] jobs)
        {
            services.AddSingleton<IJobFactory, QuartzFactory>();
            services.Add(jobs.Select(jobType => new ServiceDescriptor(jobType, jobType, ServiceLifetime.Singleton)));

            services.AddSingleton(provider =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                var scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                scheduler.Start();
                return scheduler;
            });
        }
    }
}
