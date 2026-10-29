using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Data;
using PackageFlow.API.Models;
using PackageFlow.Shared.Enums;
using PackageFlow.Shared.DTOs.WarehouseManager;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PackageFlow.API.Services.WarehouseManager
{
    public class WarehouseManagerService : IWarehouseManagerService
    {
        private readonly AppDbContext _dbContext;

        public WarehouseManagerService(AppDbContext dbContext)
        {
            _dbContext = dbContext
                ?? throw new ArgumentNullException(nameof(dbContext));
        }


        public async Task<bool> RecordWarehouseArrivalAsync(string trackingNumber, int warehouseManagerId)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.",
                    nameof(trackingNumber));

            if (warehouseManagerId <= 0)
                throw new ArgumentOutOfRangeException(nameof(warehouseManagerId));

            var package = await _dbContext.Packages
                .FirstOrDefaultAsync(p =>
                    p.TrackingNumber == trackingNumber);

            // If package does not exist.
            if (package == null)
                return false;

            // Only picked up packages can arrive at the warehouse.
            if (package.Status != Shared.Enums.PackageStatus.PickedUp)
                return false;

            var now = DateTime.UtcNow;

            package.Status = Shared.Enums.PackageStatus.InWarehouse;
            package.ReceivedInWarehouseAt = now;

            var history = new PackageStatusHistory
            {
                PackageId = package.Id,
                Status = Shared.Enums.PackageStatus.InWarehouse,
                Timestamp = now,
                ChangedByUserId = warehouseManagerId,
                Note = "The warehouse manager recorded the package arrival at the warehouse."
            };

            _dbContext.PackageStatusHistories.Add(history);

            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<List<PickupAssignmentPackageResult>> GetPackagesForPickupAssignmentAsync()
        {
            return await _dbContext.Packages
                .Where(p =>
                    p.Status == PackageStatus.Registered ||
                    p.Status == PackageStatus.PickupAssigned)
                .Select(p => new PickupAssignmentPackageResult
                {
                    TrackingNumber = p.TrackingNumber,
                    Status = p.Status,
                    ScheduledPickupDate = p.ScheduledPickupDate,
                    PickupCourierId = p.PickupCourierId,
                    PickupCourierName = p.PickupCourier != null ? p.PickupCourier.FullName : null,
                    SenderName = p.SenderUser != null ? p.SenderUser.FullName : p.SenderName
                }).ToListAsync();
        }


        public async Task<List<CourierOptionResult>> GetCouriersAsync()
        {
            return await _dbContext.CourierProfiles
                .Where(cp => cp.User != null)
                .Select(cp => new CourierOptionResult
                {
                    UserId = cp.UserId,
                    FullName = cp.User!.FullName,
                    VehiclePlateNumber = cp.VehiclePlateNumber
                }).ToListAsync();
        }


        public async Task<bool> AssignPickupCourierAsync(string trackingNumber, int courierUserId, int warehouseManagerId)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.",
                    nameof(trackingNumber));

            if (courierUserId <= 0)
                throw new ArgumentOutOfRangeException(nameof(courierUserId));

            if (warehouseManagerId <= 0)
                throw new ArgumentOutOfRangeException(nameof(warehouseManagerId));

            var package = await _dbContext.Packages
                .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);

            if (package == null)
                return false;

            if (package.Status != PackageStatus.Registered &&
                package.Status != PackageStatus.PickupAssigned)
                return false;

            var courierExists = await _dbContext.CourierProfiles
                .AnyAsync(cp => cp.UserId == courierUserId);

            if (!courierExists)
                return false;

            bool isReassignment = package.Status == PackageStatus.PickupAssigned;

            var now = DateTime.UtcNow;

            package.PickupCourierId = courierUserId;
            package.Status = PackageStatus.PickupAssigned;

            var history = new PackageStatusHistory
            {
                PackageId = package.Id,
                Status = PackageStatus.PickupAssigned,
                Timestamp = now,
                ChangedByUserId = warehouseManagerId,
                Note = isReassignment
                    ? "The warehouse manager changed the assigned pickup courier."
                    : "The warehouse manager assigned a pickup courier."
            };

            _dbContext.PackageStatusHistories.Add(history);

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
