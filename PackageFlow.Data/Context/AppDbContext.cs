using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PackageFlow.Core.Models;

namespace PackageFlow.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public AppDbContext() { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageStatusHistory> PackageStatusHistories { get; set; }
        public DbSet<WarehouseCapacityLog> WarehouseCapacityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.Property(w => w.MaxCapacity).HasDefaultValue(500);

                entity.ComplexProperty(w => w.Address).IsRequired();
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasIndex(p => p.TrackingNumber).IsUnique();
                entity.HasIndex(p => p.Status);
                entity.HasIndex(p => p.ScheduledPickupDate);

                entity.Property(p => p.WeightKg).HasPrecision(6, 2);

                entity.ComplexProperty(p => p.SenderAddress).IsRequired();
                entity.ComplexProperty(p => p.DeliveryAddress).IsRequired();

                entity.HasOne(p => p.SenderUser)
                      .WithMany(u => u.SentPackages)
                      .HasForeignKey(p => p.SenderUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.RecipientUser)
                      .WithMany(u => u.ReceivedPackages)
                      .HasForeignKey(p => p.RecipientUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.PickupCourier)
                      .WithMany(u => u.PickupAssignedPackages)
                      .HasForeignKey(p => p.PickupCourierId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.DeliveryCourier)
                      .WithMany(u => u.DeliveryAssignedPackages)
                      .HasForeignKey(p => p.DeliveryCourierId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Warehouse)
                      .WithMany(w => w.StoredPackages)
                      .HasForeignKey(p => p.WarehouseId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<PackageStatusHistory>(entity =>
            {
                entity.HasIndex(h => h.Timestamp);

                entity.HasOne(h => h.Package)
                      .WithMany(p => p.StatusHistory)
                      .HasForeignKey(h => h.PackageId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.ChangedByUser)
                      .WithMany()
                      .HasForeignKey(h => h.ChangedByUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<WarehouseCapacityLog>(entity =>
            {
                entity.HasIndex(l => l.LoggedAt);

                entity.HasOne(l => l.Warehouse)
                      .WithMany(w => w.CapacityLogs)
                      .HasForeignKey(l => l.WarehouseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}