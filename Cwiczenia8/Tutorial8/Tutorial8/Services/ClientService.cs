using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientService
{
    
    private readonly string _connectionString = 
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";
    
    // 2. GET /api/clients/{id}/trips
    public async Task<List<ClientTripDto>> GetClientTripsAsync(int clientId, CancellationToken cancellationToken)
    {
        // idKlienta -> lista wycieczek klienta
        // var clientTrips = new Dictionary<int, List<ClientTripDto>>();
        var clientTrips = new List<ClientTripDto>();
        
        // Informacje z pierwszego zapytania SQL + te z tabeli Client_Trip
        
        string command = @"
            SELECT 
                cltr.IdClient,
                t.IdTrip,
                t.Name,
                t.Description,
                t.DateFrom,
                t.DateTo,
                t.MaxPeople,
                cltr.RegisteredAt,
                cltr.PaymentDate,
                c.IdCountry,
                c.Name AS CountryName
            FROM Trip t
            JOIN Client_Trip cltr ON t.IdTrip = cltr.IdTrip
            JOIN Country_Trip cttr ON t.IdTrip = cttr.IdTrip
            JOIN Country c ON c.IdCountry = cttr.IdCountry
            WHERE cltr.IdClient = @clientId
        ";

        
        await using (var conn = new SqlConnection(_connectionString))
        await using (var cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@clientId", clientId);
            await conn.OpenAsync(cancellationToken);

            using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                
                while (await reader.ReadAsync(cancellationToken))
                {
                    int idTrip = (Int32)reader["IdTrip"];
                    
                    // żeby kilka razy nie dodać tej samej wycieczki, sprawdza po id, czy taka już nie istnieje
                    bool tripExists = false;
                    foreach (var trip in clientTrips)
                    {
                        if (idTrip == trip.IdTrip)
                        {
                            tripExists = true;
                            break;
                        };
                    }

                    if (!tripExists)
                    {
                        var clientTrip = new ClientTripDto
                        {
                            IdClient = (Int32) reader["IdClient"],
                            IdTrip = idTrip,
                            RegisteredAt = (Int32) reader["RegisteredAt"],
                            PaymentDate = (Int32) reader["PaymentDate"],
                            Name = (string) reader["Name"],
                            Description = (string) reader["Description"],
                            DateFrom = (DateTime) reader["DateFrom"],
                            DateTo = (DateTime) reader["DateTo"],
                            MaxPeople = (Int32) reader["MaxPeople"]
                        };
                        clientTrips.Add(clientTrip);
                    }
                }
            }
        }
        
        return clientTrips;
    }

    // 3. POST /api/clients
    public async Task<int> AddClient(
        string firstName, 
        string lastName, 
        string email, 
        string telephone,
        string pesel,
        CancellationToken cancellationToken)
    {
        string command = @"
                            INSERT INTO Client(FirstName, LastName, Email, Telephone, Pesel)
                            OUTPUT INSERTED.IdClient
                            VALUES(@firstName, @lastName, @email, @telephone, @pesel)
                        ";

        await using var conn = new SqlConnection(_connectionString) ;
        await using var cmd = new SqlCommand(command, conn);

        cmd.Parameters.AddWithValue("@firstName", firstName);
        cmd.Parameters.AddWithValue("@lastName", lastName);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@telephone", telephone);
        cmd.Parameters.AddWithValue("@pesel", pesel);
        
        await conn.OpenAsync(cancellationToken);
        await cmd.ExecuteNonQueryAsync(cancellationToken);

        var newClientId = (int)await cmd.ExecuteScalarAsync(cancellationToken);
        var newClient = new ClientDto()
        {
            IdClient = newClientId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Telephone = telephone,
            Pesel = pesel
        };

        return newClientId;
    }
}