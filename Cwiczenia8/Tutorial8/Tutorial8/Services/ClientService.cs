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
    // Pobiera wszystkie wycieczki związane z konkretnym klientem
    public async Task<List<ClientTripDto>> GetClientTripsAsync(int clientId, CancellationToken cancellationToken)
    {
        // Lista wycieczek klienta
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
    // Zwraca nowo utworzone ID klienta
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
    
    // 4. PUT /api/clients/{id}/trips/{tripId}
    // Zarejestruje klienta na konkretną wycieczkę
    public async Task<int> RegisterClient(int clientId, int tripId, CancellationToken cancellationToken)
    {
        // 1) Sprawdzenie, czy klient i wycieczka istnieją
        // 2) Sprawdzenie, czy wycieczka nie ma już wolnych miejsc
        // 2) Dodanie klienta do tabeli Client_Trip
        string command = @"
                IF EXISTS (SELECT * FROM Client 
                            WHERE IdClient = @clientId)
                    AND EXISTS (SELECT * FROM Trip 
                                WHERE IdTrip = @tripId)
                    AND ( (SELECT COUNT(1) FROM Client_Trip
                            WHERE IdTrip = @tripId) 
                        < 
                            (SELECT TOP 1 MaxPeople FROM TRIP
                            WHERE IdTrip = @tripId) )
                BEGIN
                    INSERT INTO Client_Trip(IdClient, IdTrip, RegisteredAt, PaymentDate)
                    VALUES (@clientId, @tripId, GETDATE(), null)
                END
        ";

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, conn);

        cmd.Parameters.AddWithValue("@clientId", clientId);
        cmd.Parameters.AddWithValue("@tripId", tripId);

        await conn.OpenAsync(cancellationToken);

        int rowsAffected = await cmd.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected == 0) return -1;

        return 0;
    }

    // 5. DELETE /api/clients/{id}/trips/{tripId}
    // Usunie rejestrację klienta z wycieczki
    public async Task<int> UnregisterClient(int clientId, int tripId, CancellationToken cancellationToken)
    {
        // 1) Sprawdzenie, czy podana rejestracja istnieje
        string command = @"
                    IF EXISTS (SELECT * FROM Client_Trip 
                                WHERE IdClient = @clientId AND IdTrip = @tripId
                    )
                    BEGIN
                        DELETE FROM Client_Trip
                        WHERE IdClient = @clientId AND IdTrip = @tripId
                    END
        ";

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, conn);

        cmd.Parameters.AddWithValue("@clientId", clientId);
        cmd.Parameters.AddWithValue("@tripId", tripId);

        await conn.OpenAsync(cancellationToken);

        int rowsAffected = await cmd.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected == 0) return -1;

        return 0;
    }
}