using PackageFlow.Shared.DTOs.Courier;
using PackageFlow.Shared.DTOs.PackageHandler;

namespace PackageFlow.API.Services.Courier
{
    public interface ICourierService
    {
        Task<List<PackageResult>> GetAssignedPackagesAsync(int courierId);

        Task<bool> RecordPickupAsync(string trackingNumber, int courierId);

        Task<bool> RecordOutForDeliveryAsync(string trackingNumber, int courierId);

        Task<List<CourierShippingHistoryResult>> GetShippingHistoryAsync(int courierId, DateTime startDate, DateTime endDate);
    }
}
