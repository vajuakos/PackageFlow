namespace PackageFlow.API.Services.PackageLabelGenerator
{
    public interface IPackageLabelGeneratorService
    {
        Task<byte[]> GenerateLabelAsync(string trackingNumber);
    }
}
