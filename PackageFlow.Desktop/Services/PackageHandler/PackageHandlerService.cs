using Microsoft.Extensions.Logging;
using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.DTOs.PackageHandler;
using System.Net.Http.Json;

namespace PackageFlow.Desktop.Services.PackageHandler
{
    public class PackageHandlerService : IPackageHandlerService
    {
        private const string BaseUrl = "api/package";

        private readonly HttpClient _httpClient;
        private readonly ILogger<PackageHandlerService> _logger;

        public PackageHandlerService(HttpClient httpClient, ILogger<PackageHandlerService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        public async Task<bool> CreatePackageAsync(CreatePackageRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/create", request);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Package successfully created via API.");
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            else
            {
                _logger.LogWarning("Failed to create package. API returned status code: {StatusCode}", response.StatusCode);
                return false;
            }
        }

        public async Task<PackageResult?> GetPackageByTrackingNumberAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            _logger.LogInformation("Fetching package details by tracking number: {TrackingNumber}", cleanTracking);

            try
            {
                var result = await _httpClient.GetFromJsonAsync<PackageResult?>($"{BaseUrl}/track/{cleanTracking}");
                
                if (result is null)
                    _logger.LogWarning("Package not found on server for tracking number: {TrackingNumber}", cleanTracking);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Network or server error while fetching package by tracking number: {TrackingNumber}", cleanTracking);
                return null;
            }
        }

        public async Task<List<PackageResult>> GetPackagesForUserAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<PackageResult>>($"{BaseUrl}/my-packages");

                var packages = response ?? new List<PackageResult>();
                _logger.LogInformation("Successfully loaded {PackageCount} packages for the user.", packages.Count);

                return packages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user packages.");
                return new List<PackageResult>();
            }
        }

        public async Task<IReadOnlyList<PackageStatusHistoryResult>> GetPackageStatusHistoryAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            var url = $"{BaseUrl}/{cleanTracking}/history";

            _logger.LogInformation("Fetching status history for tracking number: {TrackingNumber}", cleanTracking);

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<PackageStatusHistoryResult>>(url);

                var history = response ?? new List<PackageStatusHistoryResult>();
                _logger.LogInformation("Successfully loaded {HistoryCount} history events for tracking number: {TrackingNumber}", history.Count, cleanTracking);

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch status history for tracking number: {TrackingNumber}", cleanTracking);
                return new List<PackageStatusHistoryResult>();
            }
        }

        public async Task UpdatePackageDetailsAsync(string trackingNumber, UpdatePackageRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(trackingNumber)) throw new ArgumentException("Tracking number is required.", nameof(trackingNumber));

            var cleanTracking = trackingNumber.Trim();
            var url = $"{BaseUrl}/{cleanTracking}";

            var response = await _httpClient.PutAsJsonAsync(url, request);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Package successfully updated. TrackingNumber: {TrackingNumber}", cleanTracking);
            }
            else
            {
                _logger.LogWarning("Failed to update package. TrackingNumber: {TrackingNumber}, Status: {StatusCode}", cleanTracking, response.StatusCode);
            }
        }
    }
}
