using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Data;
using PackageFlow.API.Models;
using PackageFlow.Shared.Enums;

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
    }
}
