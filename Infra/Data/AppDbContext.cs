using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Motorcycle> Motorcycles { get; set; }
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Motorcycle>()
            .HasIndex(m => m.Plate)
            .IsUnique();


            modelBuilder.Entity<Courier>()
            .HasIndex(c => c.Cnpj)
            .IsUnique();


            modelBuilder.Entity<Courier>()
            .HasIndex(c => c.CnhNumber)
            .IsUnique();


            modelBuilder.Entity<Rental>()
            .HasOne(r => r.Motorcycle)
            .WithMany(m => m.Rentals)
            .HasForeignKey(r => r.MotorcycleId);


            modelBuilder.Entity<Rental>()
            .HasOne(r => r.Courier)
            .WithMany()
            .HasForeignKey(r => r.CourierId);
        }
    }
}