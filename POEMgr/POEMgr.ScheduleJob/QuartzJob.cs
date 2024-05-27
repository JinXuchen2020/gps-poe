using Microsoft.IdentityModel.Logging;
using POEMgr.Repository.DBContext;
using Quartz;

namespace POEMgr.ScheduleJob
{
    public class QuartzJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;//获取job信息
            var triggerKey = context.Trigger.Key;//获取trigger信息
            switch (triggerKey.Name)
            {
                case "job.StartCheck":
                    await new MyJobs().StartCheck();
                    break;
                case "job.SendHttpRequest":
                    await new MyJobs().SendHttpRequest();
                    break;
                case "job.SendMailTest":
                    await new MyJobs().SendMailTest();
                    break;
            }
            await Task.CompletedTask;
        }
    }
}
