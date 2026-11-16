using PackageFlow.Shared.DTOs.Courier;
using PackageFlow.Shared.DTOs.PackageHandler;

namespace PackageFlow.Desktop.Services.Courier
{
    public interface ICourierService
    {
        Task<List<PackageResult>> GetAssignedPackagesAsync();
        Task<bool> RecordPickupAsync(string trackingNumber);
        Task<bool> RecordOutForDeliveryAsync(string trackingNumber);
        Task<bool> RecordDeliveryAsync(string trackingNumber);
        Task<bool> RecordFailedDeliveryAsync(string trackingNumber, string? note=null);
        Task<List<CourierShippingHistoryResult>> GetShippingHistoryAsync(DateTime startDate, DateTime endDate);
    }
}
