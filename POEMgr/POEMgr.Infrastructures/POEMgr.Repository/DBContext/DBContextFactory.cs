using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace POEMgr.Repository.DBContext
{
    internal class DBContextFactory: IDesignTimeDbContextFactory<POEContext>
    {
        public POEContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<POEContext> builder = new DbContextOptionsBuilder<POEContext>();
            builder.UseSqlServer("Data Source=159.27.184.127,14333;Initial Catalog=POEMgr_Dev;User Id=poeadmin;Password=User@123456;Encrypt=True;TrustServerCertificate=True");
            POEContext db = new POEContext(builder.Options);
            return db;
        }
    }
}
