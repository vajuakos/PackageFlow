using PackageFlow.Shared.DTOs.WarehouseManager;

namespace PackageFlow.Desktop.Services.WarehouseManager
{
    public interface IWarehouseManagerService
    {
        Task<List<WarehousePackageResult>> GetWarehousePackagesAsync();
    }
}
