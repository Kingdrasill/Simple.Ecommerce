
using Cache.Library.Core;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.App.Interfaces.Services.ServiceResolver;
using Simple.Ecommerce.Domain.Entities;
using Simple.Ecommerce.Domain.Entities.FrequencyEntity;
using Simple.Ecommerce.Domain.ValueObjects.BaseObject;
using Simple.Ecommerce.Infra;
using System.ComponentModel.DataAnnotations;

namespace Simple.Ecommerce.Api.Services.CacheServices
{
    public class CacheAheadService : BackgroundService
    {
        private readonly ICache _cache;
        private readonly IServiceScopeFactory _scopeFactory;

        public CacheAheadService(
            ICache cache,
            IServiceScopeFactory scopeFactory
        )
        {
            _cache = cache;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!(await FillCache()) && !stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("A cache ainda não foi carregada");
            }

            while (!stoppingToken.IsCancellationRequested) 
            {
                await Task.Delay(TimeSpan.FromMinutes(10));
                _cache.AdjustValues();
                await UpdateFrequencies();
            }
        }

        private async Task UpdateFrequencies()
        {
            using (var scope = _scopeFactory.CreateScope()) 
            {
                var repository = scope.ServiceProvider.GetService<ICacheFrequencyRepository>()!;

                var values = _cache.GetValues();

                foreach (var value in values)
                {
                    var getResult = await repository.GetByEntity(value["Key"].ToString()!);

                    if (getResult.IsFailure)
                    {
                        continue;
                    }

                    var entity = new CacheFrequency().Create(
                        getResult.GetValue().Id,
                        getResult.GetValue().Entity,
                        Convert.ToInt32(value["Frequency"]),
                        Convert.ToInt32(value["HoursToLive"]),
                        Convert.ToBoolean(value["Expirable"]),
                        Convert.ToBoolean(value["KeepCached"])
                    );

                    if (entity.IsFailure)
                    {
                        continue;
                    }

                    await repository.Update(entity.GetValue());
                }
            }
        }

        private async Task<bool> FillCache()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetService<ICacheFrequencyRepository>()!;
                var dbContext = scope.ServiceProvider.GetService<TesteDbContext>()!;
                var repositoryResolver = scope.ServiceProvider.GetService<IRepositoryResolver>()!;

                var frequencies = await repository.List();

                if (frequencies.IsFailure)
                {
                    Console.WriteLine("Falha ao buscar frequências de cache.");
                    return false;
                }

                var sortedFrequencies = frequencies.GetValue().OrderByDescending(f => f.Frequency);

                foreach (var f in sortedFrequencies) 
                {
                    var entityType = typeof(BaseEntity).Assembly.ExportedTypes.Where(x => x.Name == f.Entity).FirstOrDefault()!;
                    
                    if (entityType is null)
                    {
                        Console.WriteLine($"Nenhuma tipo foi encontrada para entidade {f.Entity}");
                        continue;
                    }

                    var repositoryObject = repositoryResolver.GetRepository(entityType);
                    if (repositoryObject is null) 
                    {
                        Console.WriteLine($"Nenhum repository para entidade {entityType.Name}");
                        continue;
                    }

                    var baseListRepoType = typeof(IBaseListRepository<>).MakeGenericType(entityType);
                    if (!baseListRepoType.IsInstanceOfType(repositoryObject))
                    {
                        Console.WriteLine($"Repository de {entityType.Name} não implementa IBaseListRepository");
                        continue;
                    }

                    var listMethod = baseListRepoType.GetMethod("List");
                    var listTask = (Task)listMethod!.Invoke(repositoryObject, null)!;
                    await listTask.ConfigureAwait(false);

                    var resultProperty = listTask.GetType().GetProperty("Result")!;
                    var listResult = resultProperty.GetValue(listTask);

                    var isFailureProp = listResult!.GetType().GetProperty("IsFailure")!;
                    bool isFailure = (bool)isFailureProp.GetValue(listResult)!;
                    if (isFailure)
                    {
                        Console.WriteLine($"Erro ao listar {entityType.Name}");
                        continue;
                    }

                    var getValueMethod = listResult.GetType().GetMethod("GetValue");
                    var entityList = (IEnumerable<object>)getValueMethod!.Invoke(listResult, null)!;

                    var listDict = entityList
                        .Select(item => {
                            var dict = new Dictionary<string, object>();
                            
                            foreach (var prop in item.GetType().GetProperties())
                            {
                                var value = prop.GetValue(item);

                                if (value is ValueObject vo)
                                {
                                    foreach (var voProp in vo.GetType().GetProperties())
                                    {
                                        dict[$"{prop.Name}_{voProp.Name}"] = voProp.GetValue(vo)!;
                                    }
                                }
                                else
                                {
                                    dict[prop.Name] = value!;
                                }
                            }

                            item.GetType()
                            .GetProperties()
                            .ToDictionary(
                                prop => prop.Name,
                                prop => (object)prop.GetValue(item)!
                            );

                            return dict;
                        })
                        .ToList();

                    var keyProperty = entityType!
                        .GetProperties()
                        .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Any());

                    if (keyProperty is null)
                    {
                        Console.WriteLine($"Nenhuma propriedade [Key] encontrada para entidade {f.Entity}");
                        continue;
                    }

                    var identifier = keyProperty.Name;

                    var result = _cache.AddItem(
                        f.Entity, 
                        identifier, 
                        listDict, 
                        true, 
                        f.HoursToLive, 
                        out var message, 
                        f.Expirable, 
                        f.KeepCached
                    );
                
                    if (!result)
                    {
                        Console.WriteLine($"Erro ao adicionar entidade {f.Entity} na cache: {message}");
                    }
                }

                return true;
            }
        }
    }
}
