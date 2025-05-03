using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class CountryDto
{
    public int IdCountry { get; set; }

    [StringLength(120)] 
    public string Name { get; set; }
}