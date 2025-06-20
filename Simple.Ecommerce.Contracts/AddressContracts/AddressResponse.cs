namespace Simple.Ecommerce.Contracts.AddressContracts
{
    public record AddressResponse
    (
        int Number,
        string Street,
        string Neighbourhood,
        string City,
        string Country,
        string? Complement,
        string CEP,
        int? Id = null
    );
}
