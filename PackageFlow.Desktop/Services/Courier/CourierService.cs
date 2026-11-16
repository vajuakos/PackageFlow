using Microsoft.Extensions.Logging;
using PackageFlow.Desktop.Services.PackageHandler;
using PackageFlow.Shared.DTOs.Courier;
using PackageFlow.Shared.DTOs.PackageHandler;
using System.Net.Http.Json;

namespace PackageFlow.Desktop.Services.Courier
{
    public class CourierService : ICourierService
    {
        private const string BaseUrl = "api/courier";

        private readonly HttpClient _httpClient;
        private readonly ILogger<CourierService> _logger;

        public CourierService(HttpClient httpClient, ILogger<CourierService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<List<PackageResult>> GetAssignedPackagesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<PackageResult>>($"{BaseUrl}/assigned-packages");

                var packages = response ?? new List<PackageResult>();
                _logger.LogInformation("Successfully loaded {PackageCount} assigned packages for courier.", packages.Count);

                return packages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch assigned packages for courier.");
                return new List<PackageResult>();
            }
        }

        public async Task<bool> RecordPickupAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            var url = $"{BaseUrl}/packages/{Uri.EscapeDataString(cleanTracking)}/pickup";

            try
            {
                var response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Package pickup recorded successfully. TrackingNumber: {TrackingNumber}", cleanTracking);
                    return true;
                }

                _logger.LogWarning("Failed to record package pickup. TrackingNumber: {TrackingNumber}, StatusCode: {StatusCode}", cleanTracking, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Network or server error while recording pickup for tracking number: {TrackingNumber}", cleanTracking);
                return false;
            }
        }

        public async Task<bool> RecordOutForDeliveryAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            var url = $"{BaseUrl}/packages/{Uri.EscapeDataString(cleanTracking)}/out-for-delivery";

            try
            {
                var response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Package marked as out for delivery successfully. TrackingNumber: {TrackingNumber}", cleanTracking);
                    return true;
                }

                _logger.LogWarning("Failed to mark package as out for delivery. TrackingNumber: {TrackingNumber}, StatusCode: {StatusCode}", cleanTracking, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Network or server error while marking out for delivery for tracking number: {TrackingNumber}", cleanTracking);
                return false;
            }
        }

        public async Task<bool> RecordDeliveryAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            var url = $"{BaseUrl}/packages/{Uri.EscapeDataString(cleanTracking)}/deliver";

            try
            {
                var response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Package marked as delivered successfully. TrackingNumber: {TrackingNumber}", cleanTracking);
                    return true;
                }

                _logger.LogWarning("Failed to mark package as delivered. TrackingNumber: {TrackingNumber}, StatusCode: {StatusCode}", cleanTracking, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Network or server error while marking delivered for tracking number: {TrackingNumber}", cleanTracking);
                return false;
            }
        }

        public async Task<bool> RecordFailedDeliveryAsync(string trackingNumber, string? note = null)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            var url = $"{BaseUrl}/packages/{Uri.EscapeDataString(cleanTracking)}/fail";

            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, note);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Package marked as failed delivery successfully. TrackingNumber: {TrackingNumber}", cleanTracking);
                    return true;
                }

                _logger.LogWarning("Failed to mark package as failed delivery. TrackingNumber: {TrackingNumber}, StatusCode: {StatusCode}", cleanTracking, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Network or server error while marking delivered for tracking number: {TrackingNumber}", cleanTracking);
                return false;
            }
        }

        public async Task<List<CourierShippingHistoryResult>> GetShippingHistoryAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
                throw new ArgumentException("The start date must be earlier than the end date.");

            var startParam = Uri.EscapeDataString(startDate.ToString("O"));
            var endParam = Uri.EscapeDataString(endDate.ToString("O"));
            var url = $"{BaseUrl}/shipping-history?startDate={startParam}&endDate={endParam}";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CourierShippingHistoryResult>>(url);

                var history = response ?? new List<CourierShippingHistoryResult>();
                _logger.LogInformation("Successfully loaded {HistoryCount} shipping history records for courier.", history.Count);

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch courier shipping history.");
                return new List<CourierShippingHistoryResult>();
            }
        }
    }
}
