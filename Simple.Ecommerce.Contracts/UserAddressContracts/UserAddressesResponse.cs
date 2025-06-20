using Simple.Ecommerce.Contracts.AddressContracts;

namespace Simple.Ecommerce.Contracts.UserAddressContracts
{
    public record UserAddressesResponse
    (
        int UserId,
        string Name,
        List<AddressResponse> Addresses
    );
}
