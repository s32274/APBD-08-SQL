using APBD_MockTest_1_01.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_MockTest_1_01.Services;

public class CustomersService : ICustomersService
{
    private readonly string _connectionString = 
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";
    
    // 1. GET api/customers/{id}/rentals
    public async Task<CustomerRentalDto> GetRentalsByClientIdAsync(int id, CancellationToken cancellationToken)
    {
        var customerRentalInfo = new CustomerRentalDto();
        var rentalMap = new Dictionary<int, RentalDto>();   // rental_id -> RentalDto
        
        string sqlQuery = @"
                                SELECT  c.first_name,
                                        c.last_name,
                                        r.rental_id,
                                        r.rental_date,
                                        r.return_date,
                                        s.name,
                                        ri.price_at_rental,
                                        m.title
                                FROM Customer c
                                JOIN Rental r ON c.customer_id = r.customer_id
                                JOIN Status s ON r.status_id = s.status_id
                                JOIN Rental_Item ri ON r.rental_id = ri.rental_id
                                JOIN Movie m ON ri.movie_id = m.movie_id
                                WHERE c.customer_id = @customerId;
        ";

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sqlQuery, conn);
        cmd.Parameters.AddWithValue("@customerId", id);
        await conn.OpenAsync(cancellationToken);

        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                if (string.IsNullOrEmpty(customerRentalInfo.first_name))
                {
                    customerRentalInfo.first_name = reader["first_name"].ToString();
                    customerRentalInfo.last_name = reader["last_name"].ToString();
                }

                var rentalId = (int)reader["rental_id"];
                if (rentalId == 0)
                {
                    return null;
                }

            // żeby nie dodać jednego wypożyczenia kilka razy
                if (!rentalMap.ContainsKey(rentalId))
                {
                    var rental = new RentalDto()
                    {
                        rental_id = rentalId,
                        rental_date = (DateTime)reader["rental_date"],
                        return_date = 
                            reader["return_date"] == DBNull.Value ? null : (DateTime)reader["return_date"],
                        status = reader["name"].ToString(),
                        movies = new List<MovieDto>()
                    };
                    
                    rentalMap[rentalId] = rental;
                }
                
                // dodaje film do danego wypożyczenia
                var movie = new MovieDto
                {
                    title = reader["title"].ToString(),
                    price_at_rental = (decimal)reader["price_at_rental"]
                };
                
                rentalMap[rentalId].movies.Add(movie);
            }
        }

        customerRentalInfo.rentals = rentalMap.Values.ToList();
        return customerRentalInfo;
    }

}