using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly ClientService _clientService;

    public ClientsController(ClientService clientService)
    {
        _clientService = clientService;
    }
    
    // 2. GET /api/clients/{id}/trips
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTrips(int id, CancellationToken cancellationToken)
    {
        var trips = await _clientService.GetClientTripsAsync(id, cancellationToken);

        if (trips.Equals(null) || trips.Count == 0)
        {
            return NotFound("Klient z id " + id + " nie ma żadnej przypisanej wycieczki.");
        }
        
        return Ok(trips);
    }
}