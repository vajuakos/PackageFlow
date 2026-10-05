using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.DTOs.PackageHandler;

namespace PackageFlow.API.Services.PackageHandler
{
    public interface IPackageHandlerService
    {
        Task<bool> CreatePackageAsync(CreatePackageRequest request, int? userId);

        Task<PackageResult?> GetPackageByTrackingNumberAsync(string trackingNumber);

        Task<List<PackageResult>> GetPackagesForUserAsync(int? userId);

        Task<IReadOnlyList<PackageStatusHistoryResult>> GetPackageStatusHistoryAsync(string trackingNumber);

        Task UpdatePackageDetailsAsync(string trackingNumber, UpdatePackageRequest request);
    }
}
