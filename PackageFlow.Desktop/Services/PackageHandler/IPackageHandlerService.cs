using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.DTOs.PackageHandler;

namespace PackageFlow.Desktop.Services.PackageHandler
{
    public interface IPackageHandlerService
    {
        Task<bool> CreatePackageAsync(CreatePackageRequest request);
        Task<PackageResult?> GetPackageByTrackingNumberAsync(string trackingNumber);
        Task<List<PackageResult>> GetPackagesForUserAsync();
        Task<IReadOnlyList<PackageStatusHistoryResult>> GetPackageStatusHistoryAsync(string trackingNumber);
        Task UpdatePackageDetailsAsync(string trackingNumber, UpdatePackageRequest request);
    }
}
