namespace Simple.Ecommerce.Contracts.AddressContracts
{
    public record OrderAddressResponse
    (
        int Number,
        string Street,
        string Neighbourhood,
        string City,
        string Country,
        string? Complement,
        string CEP
    );
}
