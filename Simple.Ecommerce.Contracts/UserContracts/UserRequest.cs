namespace Simple.Ecommerce.Contracts.UserContracts
{
    public record UserRequest(
        string Name,
        string Password,
        string Email,
        string PhoneNumber,
        int Id = 0
    );
}
