using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;
using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.EntityDeletionEvents;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class LoginDeletedEventHandler : IDeleteEventHandler<LoginDeletedEvent>
    {
        private readonly TesteDbContext _context;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public LoginDeletedEventHandler(
            TesteDbContext context, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _context = context;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task Handle(LoginDeletedEvent domainEvent)
        {
            var credentialVerifications = await _context.CredentialVerifications
                .Where(cv => cv.LoginId == domainEvent.LoginId)
                .ToListAsync();

            foreach (var credentialVerification in credentialVerifications)
            {
                credentialVerification.MarkAsDeleted(raiseEvent: false);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<CredentialVerification>();
        }
    }
}
