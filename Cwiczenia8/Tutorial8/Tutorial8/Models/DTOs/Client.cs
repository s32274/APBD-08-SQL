using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class Client
{
    public int IdClient { get; set; }
    // comment
    
    [StringLength(120)]
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Pesel { get; set; }
}