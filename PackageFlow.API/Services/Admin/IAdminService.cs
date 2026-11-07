using PackageFlow.Shared.DTOs.Admin;
using PackageFlow.Shared.DTOs.WarehouseManager;

namespace PackageFlow.API.Services.Admin
{
    public interface IAdminService
    {
        Task<List<AdminPackageResult>> GetPackagesAsync();

        Task<AdminPackageResult?> GetPackageAsync(string trackingNumber);

        Task<bool> UpdatePackageAsync(string trackingNumber, AdminUpdatePackageRequest request, int adminId);

        Task<bool> DeletePackageAsync(string trackingNumber);

        Task<bool> AssignPickupCourierAsync(string trackingNumber, int courierUserId, int adminId);

        Task<bool> AssignDeliveryCourierAsync(string trackingNumber, int courierUserId, int adminId);

        Task<List<CourierOptionResult>> GetCouriersAsync();

        Task<List<AdminUserResult>> GetUsersAsync(string? role);

        Task<AdminUserResult?> GetUserAsync(int userId);

        Task<bool> CreateStaffUserAsync(CreateStaffUserRequest request);

        Task<bool> UpdateUserAsync(int userId, UpdateUserRequest request);

        Task<bool> DeleteUserAsync(int userId, int adminId);

        Task<AdminStatisticsResult> GetStatisticsAsync(DateTime startDate, DateTime endDate);
    }
}
