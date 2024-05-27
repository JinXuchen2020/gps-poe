using Microsoft.EntityFrameworkCore;
using POEMgr.Repository.DbModels;

namespace POEMgr.Repository.DBContext
{
    public class POEContext : DbContext
    {
        public POEContext(DbContextOptions<POEContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Yingjie Chen\\Documents\\POETest.mdf;Integrated Security=True;Connect Timeout=30");
            }
        }
        public DbSet<Sys_User> Sys_User { get; set; }
        public DbSet<Sys_Role> Sys_Role { get; set; }
        public DbSet<Sys_UserRole> Sys_UserRole { get; set; }
        public DbSet<Poe_OperationLog> Poe_Log { get; set; }
        public DbSet<Poe_DbLog> Poe_DbLog { get; set; }
        public DbSet<Poe_SchedulerLog> Poe_SchedulerLog { get; set; }
        public DbSet<Poe_Incentive> Poe_Incentive { get; set; }
        public DbSet<Poe_CheckPoint> Poe_CheckPoint { get; set; }
        public DbSet<Poe_CheckPointStatus> Poe_CheckPointStatus { get; set; }
        public DbSet<Poe_CurrentNumber> Poe_CurrentNumber { get; set; }
        public DbSet<Poe_Customer> Poe_Customer { get; set; }
        public DbSet<Poe_MailTemplate> Poe_MailTemplate { get; set; }
        public DbSet<Poe_MailSendRecord> Poe_MailSendRecord { get; set; }
        public DbSet<Poe_Partner> Poe_Partner { get; set; }
        public DbSet<Poe_PartnerIncentive> Poe_PartnerIncentive { get; set; }
        public DbSet<Poe_POEFile> Poe_POEFile { get; set; }
        public DbSet<Poe_POERequest> Poe_POERequest { get; set; }
        public DbSet<Poe_RequestLog> Poe_RequestLog { get; set; }
        public DbSet<Poe_RequestPhase> Poe_RequestPhase { get; set; }
        public DbSet<Poe_Subscription> Poe_Subscription { get; set; }
        public DbSet<Poe_SubscriptionStatus> Poe_SubscriptionStatus { get; set; }

        public DbSet<POEMgr.Domain.Models.User> Users { get; set; }
        public DbSet<POEMgr.Domain.Models.Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
