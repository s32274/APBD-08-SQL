using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class TripDto
{
    public int IdTrip { get; set; }
    
    [StringLength(120)]
    public string Name { get; set; }
    public string Description { get; set; }
    public string DateFrom { get; set; }
    public string DateTo { get; set; }
    public int MaxPeople { get; set; }
    
    public List<CountryDto> Countries { get; set; }
}