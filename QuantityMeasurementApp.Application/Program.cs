using Microsoft.EntityFrameworkCore;
using QuantityMeasurementApp.Repositories.Context;
using QuantityMeasurementApp.Repositories.Implementations;
using QuantityMeasurementApp.Business.Services;
using QuantityMeasurementApp.Application.Services;

class Program
{
    static void Main(string[] args)
    {
        const string connectionString =
            @"Server=localhost\SQLEXPRESS;Database=QuantityMeasurementDB;Trusted_Connection=true;TrustServerCertificate=true;";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        var dbContext = new AppDbContext(options);
        
        // Ensure database is created
        dbContext.Database.EnsureCreated();

        var repository = new QuantityMeasurementEfRepository(dbContext);

        var service = new QuantityMeasurementServiceImpl(repository);
        var menu = new Menu(service);
        menu.Start();
    }
}