namespace Simple.Ecommerce.Infra.Interfaces.Generic
{
    public interface IGenericDetachRepository<T> where T : class
    {
        public void Detach(TesteDbContext context, T entity);
    }
}
