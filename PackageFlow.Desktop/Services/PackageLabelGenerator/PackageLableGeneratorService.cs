namespace PackageFlow.Desktop.Services.PackageLabelGenerator
{
    public class PackageLableGeneratorService : IPackageLableGeneratorService
    {
        private readonly HttpClient _httpClient;

        public PackageLableGeneratorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> GetLabelAsync(string trackingNumber)
        {
            var response = await _httpClient.GetAsync($"api/package/{trackingNumber}/label");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to download label. Status code: {response.StatusCode}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
