using Microsoft.Extensions.Logging;
using PackageFlow.Shared.DTOs.Admin;
using PackageFlow.Shared.DTOs.WarehouseManager;
using System.Globalization;
using System.Net.Http.Json;

namespace PackageFlow.Desktop.Services.Admin
{
    public class AdminService : IAdminService
    {
        private const string BaseUrl = "api/admin";

        private readonly HttpClient _httpClient;
        private readonly ILogger<AdminService> _logger;

        public AdminService(HttpClient httpClient, ILogger<AdminService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        public async Task<List<AdminPackageResult>> GetPackagesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<AdminPackageResult>>($"{BaseUrl}/packages");

                var packages = response ?? new List<AdminPackageResult>();
                _logger.LogInformation("Successfully loaded {PackageCount} packages for the admin.", packages.Count);

                return packages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch packages for the admin.");
                return new List<AdminPackageResult>();
            }
        }

        public async Task<AdminPackageResult?> GetPackageAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();

            try
            {
                return await _httpClient.GetFromJsonAsync<AdminPackageResult?>($"{BaseUrl}/packages/{cleanTracking}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch package for the admin. TrackingNumber: {TrackingNumber}", cleanTracking);
                return null;
            }
        }

        public async Task<bool> UpdatePackageAsync(string trackingNumber, AdminUpdatePackageRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(trackingNumber)) throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/packages/{cleanTracking}", request);

            return LogResult(response, "Package update", cleanTracking);
        }

        public async Task<bool> DeletePackageAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber)) throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/packages/{cleanTracking}");

            return LogResult(response, "Package deletion", cleanTracking);
        }

        public async Task<bool> AssignPickupCourierAsync(string trackingNumber, int courierUserId)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber)) throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            var response = await _httpClient.PutAsJsonAsync(
                $"{BaseUrl}/packages/{cleanTracking}/pickup-courier",
                new AdminAssignCourierRequest { CourierUserId = courierUserId });

            return LogResult(response, "Pickup courier assignment", cleanTracking);
        }

        public async Task<bool> AssignDeliveryCourierAsync(string trackingNumber, int courierUserId)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber)) throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            var response = await _httpClient.PutAsJsonAsync(
                $"{BaseUrl}/packages/{cleanTracking}/delivery-courier",
                new AdminAssignCourierRequest { CourierUserId = courierUserId });

            return LogResult(response, "Delivery courier assignment", cleanTracking);
        }

        public async Task<List<CourierOptionResult>> GetCouriersAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CourierOptionResult>>($"{BaseUrl}/couriers");
                return response ?? new List<CourierOptionResult>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch couriers for the admin.");
                return new List<CourierOptionResult>();
            }
        }

        public async Task<List<AdminUserResult>> GetUsersAsync(string? role = null)
        {
            var url = string.IsNullOrWhiteSpace(role)
                ? $"{BaseUrl}/users"
                : $"{BaseUrl}/users?role={Uri.EscapeDataString(role)}";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<AdminUserResult>>(url);

                var users = response ?? new List<AdminUserResult>();
                _logger.LogInformation("Successfully loaded {UserCount} users for the admin.", users.Count);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch users for the admin.");
                return new List<AdminUserResult>();
            }
        }

        public async Task<AdminUserResult?> GetUserAsync(int userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<AdminUserResult?>($"{BaseUrl}/users/{userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user for the admin. UserId: {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> CreateStaffUserAsync(CreateStaffUserRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/users", request);

            return LogResult(response, "Staff user creation", request.Email);
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/users/{userId}", request);

            return LogResult(response, "User update", userId.ToString());
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/users/{userId}");

            return LogResult(response, "User deletion", userId.ToString());
        }

        public async Task<AdminStatisticsResult?> GetStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var start = startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var end = endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            try
            {
                return await _httpClient.GetFromJsonAsync<AdminStatisticsResult>(
                    $"{BaseUrl}/statistics?startDate={start}&endDate={end}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch admin statistics. Period: {Start} - {End}", start, end);
                return null;
            }
        }

        private bool LogResult(HttpResponseMessage response, string operation, string identifier)
        {
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("{Operation} succeeded. Identifier: {Identifier}", operation, identifier);
                return true;
            }

            _logger.LogWarning("{Operation} failed. Identifier: {Identifier}, Status: {StatusCode}",
                operation, identifier, response.StatusCode);
            return false;
        }
    }
}
