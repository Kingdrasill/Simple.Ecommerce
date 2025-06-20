namespace Simple.Ecommerce.App.Interfaces.Services.ServiceResolver
{
    public interface IRepositoryResolver
    {
        object? GetRepository(Type entityType);
    }
}
