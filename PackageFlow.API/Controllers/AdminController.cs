using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PackageFlow.API.Services.Admin;
using PackageFlow.Shared.Constants;
using PackageFlow.Shared.DTOs.Admin;
using PackageFlow.Shared.DTOs.WarehouseManager;

namespace PackageFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminController : ApiControllerBase
    {
        private const int MaxStatisticsPeriodDays = 366;

        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService
                ?? throw new ArgumentNullException(nameof(adminService));
        }

        [HttpGet("packages")]
        public async Task<ActionResult<List<AdminPackageResult>>> GetPackages()
        {
            var packages = await _adminService.GetPackagesAsync();

            return Ok(packages);
        }

        [HttpGet("packages/{trackingNumber}")]
        public async Task<ActionResult<AdminPackageResult>> GetPackage(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return BadRequest("Tracking number cannot be empty.");

            var package = await _adminService.GetPackageAsync(trackingNumber);

            if (package == null)
                return NotFound($"No package found for tracking number: {trackingNumber}");

            return Ok(package);
        }

        [HttpPut("packages/{trackingNumber}")]
        public async Task<IActionResult> UpdatePackage(string trackingNumber, [FromBody] AdminUpdatePackageRequest request)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return BadRequest("Tracking number cannot be empty.");

            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var success = await _adminService.UpdatePackageAsync(trackingNumber, request, CurrentUserId.Value);

            if (!success)
                return BadRequest("The package could not be updated. Check the tracking number, " +
                    "and that the selected status has an assigned courier.");

            return Ok("Package updated successfully.");
        }

        [HttpDelete("packages/{trackingNumber}")]
        public async Task<IActionResult> DeletePackage(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return BadRequest("Tracking number cannot be empty.");

            var success = await _adminService.DeletePackageAsync(trackingNumber);

            if (!success)
                return NotFound($"No package found for tracking number: {trackingNumber}");

            return Ok("Package deleted successfully.");
        }

        [HttpPut("packages/{trackingNumber}/pickup-courier")]
        public async Task<IActionResult> AssignPickupCourier(string trackingNumber, [FromBody] AdminAssignCourierRequest request)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return BadRequest("Tracking number cannot be empty.");

            if (request.CourierUserId <= 0)
                return BadRequest("Invalid courier user ID.");

            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var success = await _adminService.AssignPickupCourierAsync(
                trackingNumber, request.CourierUserId, CurrentUserId.Value);

            if (!success)
                return BadRequest("The pickup courier could not be assigned.");

            return Ok("Pickup courier assigned successfully.");
        }

        [HttpPut("packages/{trackingNumber}/delivery-courier")]
        public async Task<IActionResult> AssignDeliveryCourier(string trackingNumber, [FromBody] AdminAssignCourierRequest request)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return BadRequest("Tracking number cannot be empty.");

            if (request.CourierUserId <= 0)
                return BadRequest("Invalid courier user ID.");

            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var success = await _adminService.AssignDeliveryCourierAsync(
                trackingNumber, request.CourierUserId, CurrentUserId.Value);

            if (!success)
                return BadRequest("The delivery courier could not be assigned.");

            return Ok("Delivery courier assigned successfully.");
        }

        [HttpGet("couriers")]
        public async Task<ActionResult<List<CourierOptionResult>>> GetCouriers()
        {
            var couriers = await _adminService.GetCouriersAsync();

            return Ok(couriers);
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<AdminUserResult>>> GetUsers([FromQuery] string? role)
        {
            var users = await _adminService.GetUsersAsync(role);

            return Ok(users);
        }

        [HttpGet("users/{userId:int}")]
        public async Task<ActionResult<AdminUserResult>> GetUser(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");

            var user = await _adminService.GetUserAsync(userId);

            if (user == null)
                return NotFound($"No user found with ID: {userId}");

            return Ok(user);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateStaffUser([FromBody] CreateStaffUserRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request payload");

            var success = await _adminService.CreateStaffUserAsync(request);

            if (!success)
                return BadRequest("The user could not be created. The e-mail may already be in use, " +
                    "or the vehicle plate number is missing for a courier.");

            return Ok("User created successfully.");
        }

        [HttpPut("users/{userId:int}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequest request)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");

            var success = await _adminService.UpdateUserAsync(userId, request);

            if (!success)
                return BadRequest("The user could not be updated. The e-mail may already be in use.");

            return Ok("User updated successfully.");
        }

        [HttpDelete("users/{userId:int}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");

            if (CurrentUserId == null)
                return Unauthorized("Unauthorized access!");

            var success = await _adminService.DeleteUserAsync(userId, CurrentUserId.Value);

            if (!success)
                return BadRequest("The user could not be deleted. You cannot delete yourself, " +
                    "and couriers with active packages must be reassigned first.");

            return Ok("User deleted successfully.");
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<AdminStatisticsResult>> GetStatistics(
            [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate.Date > endDate.Date)
                return BadRequest("The start date must be earlier than the end date.");

            if ((endDate.Date - startDate.Date).TotalDays > MaxStatisticsPeriodDays)
                return BadRequest($"The period cannot be longer than {MaxStatisticsPeriodDays} days.");

            var statistics = await _adminService.GetStatisticsAsync(startDate, endDate);

            return Ok(statistics);
        }
    }
}
