using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface ITripsService
{
    Task<List<TripDto>> GetTripsAsync(CancellationToken cancellationToken);
}