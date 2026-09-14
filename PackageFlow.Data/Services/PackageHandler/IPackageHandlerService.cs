using PackageFlow.Core.DTOs;
using PackageFlow.Core.Models;

namespace PackageFlow.Data.Services.PackageHandler
{
    public interface IPackageHandlerService
    {
        void CreatePackage(CreatePackageRequest request);

        Package? GetPackageByTrackingNumber(string trackingNumber);
    }
}
