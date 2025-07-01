using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.Infra.Interfaces.Generic
{
    public interface IGenericDeleteRepository<T> where T : class
    {
        Task<Result<bool>> Delete(TesteDbContext context, int id);
    }
}
