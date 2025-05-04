using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
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
    
    // 3. POST /api/clients
    // Zwraca nowo utworzone ID klienta
    [HttpPost("clients")]
    public async Task<IActionResult> AddClient(
        string firstName,
        string lastName,
        string email,
        string telephone,
        string pesel,
        CancellationToken cancellationToken
    )
    {
        if (!(ClientDto.ValidateFirstName(firstName)
              && ClientDto.ValidateLastName(lastName)
              && ClientDto.ValidateEmail(email)
              && ClientDto.ValidateTelephone(telephone)
              && ClientDto.ValidatePesel(pesel)
            ))
        { 
            return BadRequest("Błędny format danych wejściowych.");
        }

        var newClientId =
            await _clientService.AddClient(firstName, lastName, email, telephone, pesel, cancellationToken);
        
        return Ok("ID nowego klienta = " + newClientId);
    }
}