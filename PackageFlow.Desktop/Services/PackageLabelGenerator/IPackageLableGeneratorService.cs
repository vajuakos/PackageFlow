namespace PackageFlow.Desktop.Services.PackageLabelGenerator
{
    public interface IPackageLableGeneratorService
    {
        Task<byte[]> GetLabelAsync(string trackingNumber);
    }
}
