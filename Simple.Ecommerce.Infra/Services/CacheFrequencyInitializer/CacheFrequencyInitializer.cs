using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.FrequencyEntity;

namespace Simple.Ecommerce.Infra.Services.CacheFrequencyInitializer
{
    public class CacheFrequencyInitializer : ICacheFrequencyInitializer
    {
        private readonly TesteDbContext _context;

        public CacheFrequencyInitializer(
            TesteDbContext context
        )
        {
            _context = context;
        }

        public async Task Initialize()
        {
            var entities = _context.Model
                .GetEntityTypes()
                .Select(t => t.ClrType.Name)
                .Distinct()
                .Where(t => t != null && t != "CacheFrequency" && t != "CredentialVerification" && t != "Coupon")
                .ToList();

            var alreadySaved = await _context.Set<CacheFrequency>()
                .Select(cf => cf.Entity)
                .ToListAsync();

            foreach (var entity in entities) 
            {
                if (!alreadySaved.Contains(entity))
                {
                    var frequency = new CacheFrequency().Create(0, entity, 0, null);
                    if (frequency.IsSuccess)
                    {
                        await _context.Set<CacheFrequency>().AddAsync(frequency.GetValue());
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
