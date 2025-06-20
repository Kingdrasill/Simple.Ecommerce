using Simple.Ecommerce.Contracts.AddressContracts;

namespace Simple.Ecommerce.Contracts.UserAddressContracts
{
    public record UserAddressRequest
    (
        int UserId,
        AddressRequest Address
    );
}
