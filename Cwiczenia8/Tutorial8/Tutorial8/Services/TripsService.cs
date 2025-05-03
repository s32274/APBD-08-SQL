using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = 
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";
    
    // 1. GET /api/trips
    public async Task<List<TripDto>> GetTripsAsync(CancellationToken cancellationToken)
    {
        // idWycieczki -> wycieczka (zawiera listę krajów)
        var tripsDict = new Dictionary<int, TripDto>();

        // dzięki JOIN'om zapytanie zapamiętuje kraje związane z konkretną wycieczką
        string command = @"
            SELECT 
                t.IdTrip, 
                t.Name, 
                t.Description, 
                t.DateFrom, 
                t.DateTo, 
                t.MaxPeople, 
                c.Name AS CountryName, 
                c.IdCountry
            FROM Trip t
            JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
            JOIN Country c ON c.IdCountry = ct.IdCountry
        ";
        
        await using (SqlConnection conn = new SqlConnection(_connectionString))
        await using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int tripId = reader.GetInt32(reader.GetOrdinal("IdTrip"));
                    
                    int countryId = reader.GetInt32(reader.GetOrdinal("IdCountry"));
                    string countryName = reader.GetString(reader.GetOrdinal("CountryName"));
                    var country = new CountryDto()
                    {
                        IdCountry = countryId,
                        Name = countryName
                    };

                    // jeśli nie ma jeszcze wycieczki, to dodaje ją do mapy
                    if (!tripsDict.ContainsKey(tripId))
                    {
                        var trip = new TripDto()
                        {
                            IdTrip = tripId,
                            Name = (string) reader["Name"],
                            Description = (string) reader["Description"],
                            DateFrom = (DateTime) reader["DateFrom"],
                            DateTo = (DateTime) reader["DateTo"],
                            MaxPeople = (Int32) reader["MaxPeople"],
                            Countries = new List<CountryDto>()
                        };

                        trip.Countries.Add(country);
                        tripsDict.Add(tripId, trip);
                    }
                    else
                    {
                        // dodaje kraj do już istniejącej listy krajów wycieczki
                        tripsDict[tripId].Countries.Add(country);
                    }
                }
            }
        }
        
        return tripsDict.Values.ToList();
    }
}