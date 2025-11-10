using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        // DbSets
        public DbSet<Motorcycle> Motorcycles { get; set; } = null!;
        public DbSet<Courier> Couriers { get; set; } = null!;
        public DbSet<Rental> Rentals { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===========================
            // Motorcycle
            // ===========================
            modelBuilder.Entity<Motorcycle>()
                .HasIndex(m => m.Plate)
                .IsUnique();

            modelBuilder.Entity<Motorcycle>()
                .Property(m => m.Model)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Motorcycle>()
                .Property(m => m.Plate)
                .IsRequired()
                .HasMaxLength(10);

            // ===========================
            // Courier
            // ===========================
            modelBuilder.Entity<Courier>()
                .HasIndex(c => c.Cnpj)
                .IsUnique();

            modelBuilder.Entity<Courier>()
                .HasIndex(c => c.CnhNumber)
                .IsUnique();

            modelBuilder.Entity<Courier>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Courier>()
                .Property(c => c.CnhCategory)
                .HasMaxLength(10);

            // ===========================
            // Rental
            // ===========================
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Motorcycle)
                .WithMany(m => m.Rentals)
                .HasForeignKey(r => r.MotorcycleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Courier)
                .WithMany()
                .HasForeignKey(r => r.CourierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .Property(r => r.DailyRate)
                .HasPrecision(10, 2);
        }
    }
}
