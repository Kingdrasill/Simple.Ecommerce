namespace Simple.Ecommerce.App.Interfaces.ReadData.BaseReadModelRepository
{
    public interface IReadModelRepository<TReadModel, TId> where TReadModel : class
    {
        Task<TReadModel> GetById(TId id);
        Task InsertOne(TReadModel readModel);
        Task Upsert(TReadModel readModel);
        Task Delete(TId id);
    }
}
