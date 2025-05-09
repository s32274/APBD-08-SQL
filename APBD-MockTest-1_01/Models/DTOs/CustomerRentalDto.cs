using System.ComponentModel.DataAnnotations;

namespace APBD_MockTest_1_01.Models.DTOs;


public class CustomerRentalDto
{
    // Customer
    [StringLength(100)]
    public string first_name { get; set; }
    [StringLength(200)]
    public string last_name { get; set; }
    
    public List<RentalDto> rentals { get; set; }
}


public class RentalDto
{
    // Rental
    public int rental_id { get; set; }
    public DateTime rental_date { get; set; }
    public DateTime? return_date { get; set; }
    
    // Status
    public string status { get; set; }
    
    public List<MovieDto> movies { get; set; }
}


public class MovieDto
{
    // Movie
    [StringLength(200)]
    public string title { get; set; }
    public Decimal price_at_rental { get; set; }
}

