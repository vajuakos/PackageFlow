using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.DTOs.PackageHandler;
using System.Net.Http.Json;

namespace PackageFlow.Desktop.Services.PackageHandler
{
    public class PackageHandlerService : IPackageHandlerService
    {
        private const string BaseUrl = "api/package";

        private readonly HttpClient _httpClient;

        public PackageHandlerService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<bool> CreatePackageAsync(CreatePackageRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/create", request);

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<PackageResult?> GetPackageByTrackingNumberAsync(string trackingNumber)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<PackageResult?>($"{BaseUrl}/track/{trackingNumber.Trim()}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<PackageResult>> GetPackagesForUserAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<PackageResult>>($"{BaseUrl}/my-packages");

                return response ?? new List<PackageResult>();
            }
            catch (Exception)
            {
                return new List<PackageResult>();
            }
        }

        public async Task<IReadOnlyList<PackageStatusHistoryResult>> GetPackageStatusHistoryAsync(string trackingNumber)
        {
            try
            {
                var url = $"{BaseUrl}/{trackingNumber.Trim()}/history";

                var response = await _httpClient.GetFromJsonAsync<List<PackageStatusHistoryResult>>(url);

                return response ?? new List<PackageStatusHistoryResult>();
            }
            catch (Exception)
            {
                return new List<PackageStatusHistoryResult>();
            }
        }

        public async Task UpdatePackageDetailsAsync(string trackingNumber, UpdatePackageRequest request)
        {
            var url = $"{BaseUrl}/{trackingNumber.Trim()}";

            await _httpClient.PutAsJsonAsync(url, request);
        }
    }
}
