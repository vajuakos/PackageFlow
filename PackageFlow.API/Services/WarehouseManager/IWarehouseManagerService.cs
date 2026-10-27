using PackageFlow.Shared.DTOs.PackageHandler;

namespace PackageFlow.API.Services.WarehouseManager
{
    public interface IWarehouseManagerService
    {
        Task<bool> RecordWarehouseArrivalAsync(string trackingNumber, int warehouseManagerId);
    }
}
