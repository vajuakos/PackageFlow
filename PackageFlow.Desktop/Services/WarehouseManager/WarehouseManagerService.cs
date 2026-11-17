using Microsoft.Extensions.Logging;
using PackageFlow.Shared.DTOs.WarehouseManager;
using System.Net.Http.Json;

namespace PackageFlow.Desktop.Services.WarehouseManager
{
    public class WarehouseManagerService : IWarehouseManagerService
    {
        private const string BaseUrl = "api/warehousemanager";

        private readonly HttpClient _httpClient;
        private readonly ILogger<WarehouseManagerService> _logger;

        public WarehouseManagerService(HttpClient httpClient, ILogger<WarehouseManagerService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        public async Task<List<WarehousePackageResult>> GetWarehousePackagesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<WarehousePackageResult>>($"{BaseUrl}/packages");

                var packages = response ?? new List<WarehousePackageResult>();
                _logger.LogInformation("Successfully loaded {PackageCount} warehouse packages.", packages.Count);

                return packages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch warehouse packages.");
                return new List<WarehousePackageResult>();
            }
        }

        public async Task<bool> RecordWarehouseArrivalAsync(string trackingNumber)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseUrl}/packages/{trackingNumber}/warehouse-arrival", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record warehouse arrival for {TrackingNumber}.", trackingNumber);
                return false;
            }
        }

        public async Task<bool> RecordWarehouseReturnAsync(string trackingNumber)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseUrl}/packages/{trackingNumber}/warehouse-return", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record warehouse return for {TrackingNumber}.", trackingNumber);
                return false;
            }
        }

        public async Task<List<PickupAssignmentPackageResult>> GetPackagesForPickupAssignmentAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<PickupAssignmentPackageResult>>($"{BaseUrl}/pickup-assignment/packages");
                return response ?? new List<PickupAssignmentPackageResult>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch packages for pickup assignment.");
                return new List<PickupAssignmentPackageResult>();
            }
        }

        public async Task<List<CourierOptionResult>> GetCouriersAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CourierOptionResult>>($"{BaseUrl}/pickup-assignment/couriers");
                return response ?? new List<CourierOptionResult>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch couriers.");
                return new List<CourierOptionResult>();
            }
        }

        public async Task<bool> AssignPickupCourierAsync(string trackingNumber, int courierUserId)
        {
            try
            {
                var request = new AssignPickupCourierRequest { CourierUserId = courierUserId };
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/pickup-assignment/packages/{trackingNumber}/courier", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign pickup courier for {TrackingNumber}.", trackingNumber);
                return false;
            }
        }

        public async Task<List<DeliveryAssignmentPackageResult>> GetPackagesForDeliveryAssignmentAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<DeliveryAssignmentPackageResult>>($"{BaseUrl}/delivery-assignment/packages");
                return response ?? new List<DeliveryAssignmentPackageResult>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch packages for delivery assignment.");
                return new List<DeliveryAssignmentPackageResult>();
            }
        }

        public async Task<bool> AssignDeliveryCourierAsync(string trackingNumber, int courierUserId)
        {
            try
            {
                var request = new AssignDeliveryCourierRequest { CourierUserId = courierUserId };
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/delivery-assignment/packages/{trackingNumber}/courier", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign delivery courier for {TrackingNumber}.", trackingNumber);
                return false;
            }
        }
    }
}
