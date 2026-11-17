using PackageFlow.Shared.DTOs.WarehouseManager;

namespace PackageFlow.Desktop.Services.WarehouseManager
{
    public interface IWarehouseManagerService
    {
        Task<List<WarehousePackageResult>> GetWarehousePackagesAsync();
        Task<bool> RecordWarehouseArrivalAsync(string trackingNumber);
        Task<bool> RecordWarehouseReturnAsync(string trackingNumber);
        Task<List<PickupAssignmentPackageResult>> GetPackagesForPickupAssignmentAsync();
        Task<List<CourierOptionResult>> GetCouriersAsync();
        Task<bool> AssignPickupCourierAsync(string trackingNumber, int courierUserId);
        Task<List<DeliveryAssignmentPackageResult>> GetPackagesForDeliveryAssignmentAsync();
        Task<bool> AssignDeliveryCourierAsync(string trackingNumber, int courierUserId);
    }
}
