using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Simple.Ecommerce.App.Interfaces.ReadData.BaseReadModelRepository;
using System.Reflection;

namespace Simple.Ecommerce.Infra.ReadModelRepositories.BaseReadModelRepository
{
    public class MongoDBReadModelRepository<TReadModel, TId> : IReadModelRepository<TReadModel, TId> where TReadModel : class
    {
        protected readonly IMongoCollection<TReadModel> _collection;

        public MongoDBReadModelRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<TReadModel>(collectionName);
        }

        public async Task<TReadModel> GetById(TId id)
        {
            var filter = Builders<TReadModel>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task InsertOne(TReadModel readModel)
        {
            await _collection.InsertOneAsync(readModel);
        }

        public async Task Upsert(TReadModel readModel)
        {
            var idProperty = typeof(TReadModel).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<BsonIdAttribute>() != null) ??
                typeof(TReadModel).GetProperty("Id") ??
                typeof(TReadModel).GetProperty("id");

            if (idProperty == null)
            {
                throw new InvalidOperationException($"Cannot perform upsert: Type {typeof(TReadModel).Name} does not have a property marked with [BsonId] or named 'Id'/'id'.");
            }

            TId idValue = (TId)idProperty.GetValue(readModel)!;

            var filter = Builders<TReadModel>.Filter.Eq("_id", idValue);
            var options = new ReplaceOptions { IsUpsert = true };

            await _collection.ReplaceOneAsync(filter, readModel, options);
        }

        public async Task Delete(TId id)
        {
            var filter = Builders<TReadModel>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}
