using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripsService _tripsService;

        public TripsController(ITripsService tripsService)
        {
            _tripsService = tripsService;
        }

        // 1. GET /api/trips
        // Pobiera wszystkie dostępne wycieczki wraz z ich podstawowymi informacjami
        [HttpGet]
        public async Task<IActionResult> GetTrips(CancellationToken cancellationToken)
        {
            var trips = await _tripsService.GetTripsAsync(cancellationToken);
            return Ok(trips);
        }

        // 1. GET /api/trips
        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetTrip(int id)
        // {
        //     // if( await DoesTripExist(id)){
        //     //  return NotFound();
        //     // }
        //     // var trip = ... GetTrip(id);
        //     return Ok();
        // }
        
        
    }
}
