using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PackageFlow.API.Services.Courier;
using PackageFlow.Shared.Constants;
using PackageFlow.Shared.DTOs.PackageHandler;
using PackageFlow.Shared.DTOs.Common;
using PackageFlow.Shared.DTOs.Courier;


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
                return Unauthorized("Unauthorized access!");

            var success = await _courierService
                .RecordPickupAsync(trackingNumber, CurrentUserId.Value);

            if (!success)
                return BadRequest("The package could not be picked up. Check the tracking number," +
                    "assignment, and package status.");

            return Ok("Package pickup recorded successfully.");
        }


        [HttpPost("packages/{trackingNumber}/out-for-delivery")]
        public async Task<ActionResult> RecordOutForDelivery(string trackingNumber)
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var success = await _courierService
                .RecordOutForDeliveryAsync(trackingNumber, CurrentUserId.Value);

            if (!success)
                return BadRequest("The package could not be marked as out for delivery.");

            return Ok("Package marked as out for delivery successfully.");
        }


        [HttpGet("shipping-history")]
        public async Task<ActionResult<List<CourierShippingHistoryResult>>> GetShippingHistory(
            [FromQuery]DateTime startDate, [FromQuery]DateTime endDate)
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            if (startDate >= endDate)
                return BadRequest("The start date must be earlier than the end date.");

            var history = await _courierService.GetShippingHistoryAsync(
                CurrentUserId.Value, startDate, endDate);

            return Ok(history);
        }
    }
}
