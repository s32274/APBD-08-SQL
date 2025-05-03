using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class TripDto
{
    public int IdTrip { get; set; }
    
    [StringLength(120)]
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    
    // "Dołącz informacje o krajach dla każdej wycieczki"
    public List<CountryDto> Countries { get; set; }
}