using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PackageFlow.API.Services.WarehouseManager;
using PackageFlow.Shared.Constants;


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
                return Unauthorized("Unathorized access!");

            var success = await _warehouseManagerService
                .RecordWarehouseArrivalAsync(trackingNumber, CurrentUserId.Value);

            if (!success)
                return BadRequest("The warehouse arrival could not be recorded.");

            return Ok("Package warehouse arrival recorded successfully.");
        }

    }
}
