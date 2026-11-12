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
    }
}
