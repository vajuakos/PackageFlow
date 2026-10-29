using PackageFlow.Shared.DTOs.PackageHandler;
using PackageFlow.Shared.DTOs.WarehouseManager;

namespace PackageFlow.API.Services.WarehouseManager
{
    public interface IWarehouseManagerService
    {
        Task<bool> RecordWarehouseArrivalAsync(string trackingNumber, int warehouseManagerId);

        Task<List<PickupAssignmentPackageResult>> GetPackagesForPickupAssignmentAsync();

        Task<List<CourierOptionResult>> GetCouriersAsync();

        Task<bool> AssignPickupCourierAsync(string trackingNumber, int courierUserId, int warehouseManagerId);
    }
}
