using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PackageFlow.API.Services.WarehouseManager;
using PackageFlow.Shared.Constants;
using PackageFlow.Shared.DTOs.WarehouseManager;
using System.Security.Claims;


namespace PackageFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.WarehouseManager)]
    public class WarehouseManagerController : ApiControllerBase
    {
        private readonly IWarehouseManagerService _warehouseManagerService;

        public WarehouseManagerController(IWarehouseManagerService warehouseManagerService)
        {
            _warehouseManagerService = warehouseManagerService
                ?? throw new ArgumentNullException(nameof(warehouseManagerService));
        }

        [HttpPost("packages/{trackingNumber}/warehouse-arrival")]
        public async Task<ActionResult> RecordWarehouseArrival(string trackingNumber)
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var success = await _warehouseManagerService
                .RecordWarehouseArrivalAsync(trackingNumber, CurrentUserId.Value);

            if (!success)
                return BadRequest("The warehouse arrival could not be recorded.");

            return Ok("Package warehouse arrival recorded successfully.");
        }


        [HttpGet("pickup-assignment/packages")]
        public async Task<ActionResult<List<PickupAssignmentPackageResult>>> GetPackagesForPickupAssignment()
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var packages = await _warehouseManagerService
                .GetPackagesForPickupAssignmentAsync();

            return Ok(packages);
        }


        [HttpGet("pickup-assignment/couriers")]
        public async Task<ActionResult<List<CourierOptionResult>>> GetCouriers()
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var couriers = await _warehouseManagerService
                .GetCouriersAsync();

            return Ok(couriers);
        }


        [HttpPut("pickup-assignment/packages/{trackingNumber}/courier")]
        public async Task<IActionResult> AssignPickupCourier(string trackingNumber, [FromBody]AssignPickupCourierRequest request)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return BadRequest("Tracking number cannot be empty.");

            if (request.CourierUserId <= 0)
                return BadRequest("Invalid courier user ID.");

            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var success = await _warehouseManagerService
                .AssignPickupCourierAsync(
                    trackingNumber,
                    request.CourierUserId,
                    CurrentUserId.Value);

            if (!success)
                return BadRequest("The package could not be assigned.");

            return Ok("Pickup courier assigned successfully.");
        }


        [HttpGet("delivery-assignment/packages")]
        public async Task<ActionResult<List<DeliveryAssignmentPackageResult>>> GetPackagesForDeliveryAssignment()
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var packages = await _warehouseManagerService
                .GetPackagesForDeliveryAssignmentAsync();

            return Ok(packages);
        }


        [HttpGet("delivery-assignment/couriers")]
        public async Task<ActionResult<List<CourierOptionResult>>> GetCouriersForDeliveryAssignment()
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var couriers = await _warehouseManagerService
                .GetCouriersAsync();

            return Ok(couriers);
        }


        [HttpPut("delivery-assignment/packages/{trackingNumber}/courier")]
        public async Task<IActionResult> AssignDeliveryCourier(string trackingNumber, [FromBody] AssignDeliveryCourierRequest request)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return BadRequest("Tracking number cannot be empty.");

            if (request.CourierUserId <= 0)
                return BadRequest("Invalid courier user ID.");

            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var success = await _warehouseManagerService
                .AssignDeliveryCourierAsync(
                    trackingNumber,
                    request.CourierUserId,
                    CurrentUserId.Value);

            if (!success)
                return BadRequest("The package could not be assigned.");

            return Ok("Delivery courier assigned successfully.");
        }

    }
}
