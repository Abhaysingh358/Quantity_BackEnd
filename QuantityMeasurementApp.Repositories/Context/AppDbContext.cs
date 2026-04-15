using Microsoft.EntityFrameworkCore;
using QuantityMeasurementApp.Models.Entities; 

namespace QuantityMeasurementApp.Repositories.Context
{
    // AppDbContext is the bridge between your C# code and SQL Server.
    // EF Core uses this class to:
    //   1. Know which tables exist  (DbSet<T> properties)
    //   2. Configure column names, constraints, max lengths (OnModelCreating)
    //   3. Run migrations (dotnet ef migrations add ...)

    public class AppDbContext : DbContext
    {
        // This constructor accepts options from outside (connection string etc.)
        // It lets you pass different configs in production vs tests.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // This property = the MeasurementHistory table in SQL Server.
        // EF Core will create / migrate the table to match MeasurementHistoryRecord.
        public DbSet<MeasurementHistoryRecord> MeasurementHistory { get; set; }

        // Users table — used for authentication
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // MeasurementHistory table 
            modelBuilder.Entity<MeasurementHistoryRecord>(entity =>
            {
                entity.ToTable("MeasurementHistory");

                // Primary key
                entity.HasKey(e => e.Id);

                // Required string columns — explicit max length avoids NVARCHAR(MAX) defaults
                entity.Property(e => e.Operation)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.Operand1Unit)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.Operand1MeasurementType)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.ResultType)
                      .IsRequired()
                      .HasMaxLength(20);

                // Optional string columns
                entity.Property(e => e.Operand2Unit).HasMaxLength(50);
                entity.Property(e => e.Operand2MeasurementType).HasMaxLength(50);
                entity.Property(e => e.ResultQuantityUnit).HasMaxLength(50);
                entity.Property(e => e.ResultQuantityMeasurementType).HasMaxLength(50);
                entity.Property(e => e.ErrorMessage).HasMaxLength(500);

                // CreatedAt — SQL Server sets this automatically on INSERT
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("NOW()");

                // FK — links each history record to a user
                // SetNull means if the user is deleted, history stays but UserId becomes null
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            //  Users table 
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.HasIndex(u => u.Email).IsUnique();   // no duplicate emails

                entity.Property(u => u.PasswordHash).HasMaxLength(512);
                entity.Property(u => u.GoogleId).HasMaxLength(128);
            });
        }
    }
}