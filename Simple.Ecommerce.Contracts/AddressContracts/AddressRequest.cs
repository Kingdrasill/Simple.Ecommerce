namespace Simple.Ecommerce.Contracts.AddressContracts
{
    public record AddressRequest
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
