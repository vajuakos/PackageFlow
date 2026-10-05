using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Models;
using PackageFlow.Shared.Enums;

namespace PackageFlow.API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CourierProfile> CourierProfiles { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageStatusHistory> PackageStatusHistories { get; set; }
        public DbSet<WarehouseCapacityLog> WarehouseCapacityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.ComplexProperty(u => u.DefaultAddress, a =>
                {
                    a.Property(p => p.Country).HasMaxLength(60);
                    a.Property(p => p.City).HasMaxLength(100);
                    a.Property(p => p.PostalCode).HasMaxLength(20);
                    a.Property(p => p.Street).HasMaxLength(150);
                    a.Property(p => p.StreetNumber).HasMaxLength(20);
                    a.Property(p => p.State).HasMaxLength(100);
                    a.Property(p => p.BuildingDetails).HasMaxLength(100);
                });

                entity.HasOne(u => u.CourierProfile)
                      .WithOne(c => c.User)
                      .HasForeignKey<CourierProfile>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CourierProfile>(entity =>
            {
                entity.HasIndex(c => c.UserId).IsUnique();
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.Property(w => w.MaxCapacity).HasDefaultValue(500);

                entity.ComplexProperty(w => w.Address, a =>
                {
                    a.Property(p => p.Country).HasMaxLength(60);
                    a.Property(p => p.City).HasMaxLength(100);
                    a.Property(p => p.PostalCode).HasMaxLength(20);
                    a.Property(p => p.Street).HasMaxLength(150);
                    a.Property(p => p.StreetNumber).HasMaxLength(20);
                    a.Property(p => p.State).HasMaxLength(100);
                    a.Property(p => p.BuildingDetails).HasMaxLength(100);
                });
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasIndex(p => p.TrackingNumber).IsUnique();
                entity.HasIndex(p => p.Status);
                entity.HasIndex(p => p.ScheduledPickupDate);

                entity.Property(p => p.WeightKg).HasPrecision(6, 2);

                entity.Property(p => p.Status)
                      .HasConversion<string>()
                      .HasMaxLength(30)
                      .HasDefaultValue(PackageStatus.Registered);

                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(p => p.Size)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.ComplexProperty(p => p.SenderAddress, a =>
                {
                    a.Property(x => x.Country).HasMaxLength(60);
                    a.Property(x => x.City).HasMaxLength(100);
                    a.Property(x => x.PostalCode).HasMaxLength(20);
                    a.Property(x => x.Street).HasMaxLength(150);
                    a.Property(x => x.StreetNumber).HasMaxLength(20);
                    a.Property(x => x.State).HasMaxLength(100);
                    a.Property(x => x.BuildingDetails).HasMaxLength(100);
                });

                entity.ComplexProperty(p => p.DeliveryAddress, a =>
                {
                    a.Property(x => x.Country).HasMaxLength(60);
                    a.Property(x => x.City).HasMaxLength(100);
                    a.Property(x => x.PostalCode).HasMaxLength(20);
                    a.Property(x => x.Street).HasMaxLength(150);
                    a.Property(x => x.StreetNumber).HasMaxLength(20);
                    a.Property(x => x.State).HasMaxLength(100);
                    a.Property(x => x.BuildingDetails).HasMaxLength(100);
                });

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

                entity.Property(h => h.Timestamp)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(h => h.Status)
                      .HasConversion<string>()
                      .HasMaxLength(30);

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

                entity.Property(l => l.LoggedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(l => l.Warehouse)
                      .WithMany(w => w.CapacityLogs)
                      .HasForeignKey(l => l.WarehouseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
