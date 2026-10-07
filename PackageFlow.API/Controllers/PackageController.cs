using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PackageFlow.API.Models;
using PackageFlow.API.Services.PackageHandler;
using PackageFlow.Shared.DTOs.PackageHandler;

namespace PackageFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ApiControllerBase
    {
        private readonly IPackageHandlerService _packageHandlerService;

        public PackageController(IPackageHandlerService packageHandlerService)
        {
            _packageHandlerService = packageHandlerService ?? throw new ArgumentNullException(nameof(packageHandlerService));
        }

        [HttpPost("create")]
        public async Task<ActionResult<bool>> CreatePackage([FromBody] CreatePackageRequest request)
        {
            if (request == null) return BadRequest("Invalid request payload");

            var success = await _packageHandlerService.CreatePackageAsync(request, CurrentUserId);

            if (success) return Ok(true);

            return BadRequest(false);
        }

        [AllowAnonymous]
        [HttpGet("track/{trackingNumber}")]
        public async Task<ActionResult<PackageResult>> GetPackageByTrackingNumber(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber)) return BadRequest("Tracking number must be provided!");

            var package = await _packageHandlerService.GetPackageByTrackingNumberAsync(trackingNumber);

            if (package == null) return NotFound($"No package found for tracking number: {trackingNumber}");

            return Ok(package);
        }

        [HttpGet("my-packages")]
        public async Task<ActionResult<List<PackageResult>>> GetPackagesForUser()
        {
            if (CurrentUserId == null) return Unauthorized("Unauthorized access!");

            var packages = await _packageHandlerService.GetPackagesForUserAsync(CurrentUserId);

            return Ok(packages);
        }

        [HttpGet("{trackingNumber}/history")]
        public async Task<ActionResult<IReadOnlyList<PackageStatusHistory>>> GetPackageStatusHistory(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return BadRequest("Tracking number must be provided!");

            var history = await _packageHandlerService.GetPackageStatusHistoryAsync(trackingNumber);

            if (history == null)
                return NotFound($"No package found for tracking number: {trackingNumber}");

            return Ok(history);
        }

        [HttpPut("{trackingNumber}")]
        public async Task<IActionResult> UpdatePackage(string trackingNumber, [FromBody] UpdatePackageRequest request)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return BadRequest("Tracking number must be provided!");

            try
            {
                await _packageHandlerService.UpdatePackageDetailsAsync(trackingNumber, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Package not found");
            }
        }
    }
}
