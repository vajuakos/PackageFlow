using PackageFlow.Shared.DTOs.PackageHandler;

namespace PackageFlow.API.Services.Courier
{
    public interface ICourierService
    {
        Task<List<PackageResult>> GetAssignedPackagesAsync(int courierId);

        Task<bool> RecordPickupAsync(string trackingNumber, int courierId);
    }
}
