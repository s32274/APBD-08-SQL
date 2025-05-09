using APBD_MockTest_1_01.Models.DTOs;

namespace APBD_MockTest_1_01.Services;

public interface ICustomersService
{
    public Task<CustomerRentalDto> GetRentalsByClientIdAsync(int id, CancellationToken cancellationToken);
}