using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Data;
using PackageFlow.API.Models;
using PackageFlow.Shared.Enums;
using PackageFlow.Shared.DTOs.WarehouseManager;
using PackageFlow.Shared.DTOs.Common;
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

            var warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync();
            if (warehouse == null)
                throw new InvalidOperationException("Central warehouse does not exist in the database.");
            
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
            package.WarehouseId = warehouse.Id;
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
                .AsNoTracking()
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

        public async Task<List<DeliveryAssignmentPackageResult>> GetPackagesForDeliveryAssignmentAsync()
        {
            return await _dbContext.Packages
                .AsNoTracking()
                .Where(p =>
                    p.Status == PackageStatus.InWarehouse ||
                    p.Status == PackageStatus.DeliveryAssigned)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new DeliveryAssignmentPackageResult
                {
                    TrackingNumber = p.TrackingNumber,
                    Status = p.Status,
                    ScheduledPickupDate = p.ScheduledPickupDate,
                    DeliveryCourierId = p.DeliveryCourierId,
                    DeliveryCourierName = p.DeliveryCourier != null ? p.DeliveryCourier.FullName : null,
                    RecipientName = p.RecipientUser != null ? p.RecipientUser.FullName : p.RecipientName,
                    DeliveryAddress = new AddressResult
                    {
                        Country = p.DeliveryAddress.Country,
                        State = p.DeliveryAddress.State,
                        PostalCode = p.DeliveryAddress.PostalCode,
                        City = p.DeliveryAddress.City,
                        Street = p.DeliveryAddress.Street,
                        StreetNumber = p.DeliveryAddress.StreetNumber,
                        BuildingDetails = p.DeliveryAddress.BuildingDetails
                    }
                }).ToListAsync();
        }

        public async Task<bool> AssignDeliveryCourierAsync(string trackingNumber, int courierUserId, int warehouseManagerId)
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

            if (package.Status != PackageStatus.InWarehouse &&
                package.Status != PackageStatus.DeliveryAssigned)
                return false;

            var courierExists = await _dbContext.CourierProfiles
                .AnyAsync(cp => cp.UserId == courierUserId);

            if (!courierExists)
                return false;

            bool isReassignment = package.Status == PackageStatus.DeliveryAssigned;

            var now = DateTime.UtcNow;

            package.DeliveryCourierId = courierUserId;
            package.Status = PackageStatus.DeliveryAssigned;

            var history = new PackageStatusHistory
            {
                PackageId = package.Id,
                Status = PackageStatus.DeliveryAssigned,
                Timestamp = now,
                ChangedByUserId = warehouseManagerId,
                Note = isReassignment
                    ? "The warehouse manager changed the assigned delivery courier."
                    : "The warehouse manager assigned a delivery courier."
            };

            _dbContext.PackageStatusHistories.Add(history);

            await _dbContext.SaveChangesAsync();

            return true;
        }
        
        public async Task<List<WarehousePackageResult>> GetWarehousePackagesAsync()
        {
            return await _dbContext.Packages
                .AsNoTracking()
                .Where(p =>
                    p.Status == PackageStatus.PickupAssigned ||
                    p.Status == PackageStatus.PickedUp ||
                    p.Status == PackageStatus.InWarehouse ||
                    p.Status == PackageStatus.DeliveryAssigned ||
                    p.Status == PackageStatus.OutForDelivery)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new WarehousePackageResult
                {
                    TrackingNumber = p.TrackingNumber,
                    Status = p.Status,
                    Size = p.Size,
                    WeightKg = p.WeightKg,
                    ScheduledPickupDate = p.ScheduledPickupDate,
                    SenderName = p.SenderUser != null ? p.SenderUser.FullName : p.SenderName,
                    RecipientName = p.RecipientUser != null ? p.RecipientUser.FullName : p.RecipientName,
                    DeliveryAddress = new AddressResult
                    {
                        Country = p.DeliveryAddress.Country,
                        State = p.DeliveryAddress.State,
                        PostalCode = p.DeliveryAddress.PostalCode,
                        City = p.DeliveryAddress.City,
                        Street = p.DeliveryAddress.Street,
                        StreetNumber = p.DeliveryAddress.StreetNumber,
                        BuildingDetails = p.DeliveryAddress.BuildingDetails
                    },
                    PickupCourierId = p.PickupCourierId,
                    PickupCourierName = p.PickupCourier != null ? p.PickupCourier.FullName : null,
                    DeliveryCourierId = p.DeliveryCourierId,
                    DeliveryCourierName = p.DeliveryCourier != null ? p.DeliveryCourier.FullName : null,
                    ReceivedInWarehouseAt = p.ReceivedInWarehouseAt
                }).ToListAsync();
        }
    }
}
