using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using QuantityMeasurementApp.Repositories.Context;

namespace QuantityMeasurementApp.Repositories
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql("Host=dpg-d7fre058nd3s73e3vuk0-a.oregon-postgres.render.com;Port=5432;Database=quantitymeasurementdb_usoz;Username=quantitymeasurementdb_usoz_user;Password=ilIZGEzxDmz7FGIPoodbdiWXoML4yqEn;SSL Mode=Require;Trust Server Certificate=true")
                .Options;

            return new AppDbContext(options);
        }
    }
}