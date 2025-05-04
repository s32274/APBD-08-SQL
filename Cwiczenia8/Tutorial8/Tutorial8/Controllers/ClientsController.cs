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
    // Pobiera wszystkie wycieczki związane z konkretnym klientem
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
    [HttpPost]
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
    
    // 4. PUT /api/clients/{id}/trips/{tripId}
    // Zarejestruje klienta na konkretną wycieczkę
    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClient(int clientId, int tripId, CancellationToken cancellationToken)
    {
        var result =  await _clientService.RegisterClient(clientId, tripId, cancellationToken);
        
        if (result == -1)
            return NotFound("Nie ma takiego klienta (id = " + clientId + ") lub wycieczki (id = " + tripId + ").");
        
        return Ok("Dodano klienta (id = " + clientId + ") do wycieczki (id = " + tripId + ").");
    }
    
    // 5. DELETE /api/clients/{id}/trips/{tripId}
    // Usunie rejestrację klienta z wycieczki
    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> UnregisterClient(int clientId, int tripId, CancellationToken cancellationToken)
    {
        var result = await _clientService.UnregisterClient(clientId, tripId, cancellationToken);

        if (result == -1)
            return NotFound("Nie ma rejestracji klienta o id = " + clientId + " na wycieczkę o id = " + tripId);
        
        return Ok("Usunięto rezerwację klienta o id = " + clientId + " na wycieczkę o id = " + tripId);
    }
}