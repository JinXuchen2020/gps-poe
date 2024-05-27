using MailService.Interfaces;
using MailService.Services;
using POEMgr.Repository;
using POEMgr.ScheduleJob;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRepositoryServices(builder.Configuration["ConnectionStrings:DBServer"]);
builder.Services.AddTransient<IEmailService>(provider =>
{
    var mail = builder.Configuration["NotificationEmail:Name"].ToString();
    var password = builder.Configuration["NotificationEmail:Password"].ToString();
    return new EmailServiceCore(mail, password);
});
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
CZHttpContext.ServiceProvider = builder.Services.BuildServiceProvider();
builder.Services.AddQuartz(typeof(QuartzJob));
var app = builder.Build();
QuartzService.StartJobs<QuartzJob>();
app.MapGet("/", () => "Hello World!");
app.Run();