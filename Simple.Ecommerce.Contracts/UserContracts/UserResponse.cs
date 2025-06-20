namespace Simple.Ecommerce.Contracts.UserContracts
{
    public record UserResponse(
        int Id,
        string Name,
        string Email,
        string PhoneNumber
    );
}
