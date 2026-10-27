using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PackageFlow.API.Services.Courier;
using PackageFlow.Shared.Constants;
using PackageFlow.Shared.DTOs.PackageHandler;


namespace PackageFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Courier)]
    
    public class CourierController : ApiControllerBase
    {
        private readonly ICourierService _courierService;

        public CourierController(ICourierService courierService)
        {
            _courierService = courierService
                ?? throw new ArgumentNullException(nameof(courierService));
        }


        [HttpGet("assigned-packages")]
        public async Task<ActionResult<List<PackageResult>>> GetAssignedPackages()
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var packages = await _courierService
                .GetAssignedPackagesAsync(CurrentUserId.Value);

            return Ok(packages);
        }


        [HttpPost("packages/{trackingNumber}/pickup")]
        public async Task<ActionResult> RecordPickup(string trackingNumber)
        {
            if (CurrentUserId == null)
                return Unauthorized("Unathorized access!");

            var success = await _courierService
                .RecordPickupAsync(trackingNumber, CurrentUserId.Value);

            if (!success)
                return BadRequest("The package could not be pickud up. Check the tracking number," +
                    "assignment, and package status.");

            return Ok("Package pickup recorded successfully.");
        }
    }
}
