using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;


namespace CHNU_Connect.DAL.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql("Host=ep-aged-butterfly-ad72v8le-pooler.c-2.us-east-1.aws.neon.tech;Database=chnu_connectdb;Username=neondb_owner;Password=npg_x7rvt1Lkahyd;SSL Mode=Require");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
