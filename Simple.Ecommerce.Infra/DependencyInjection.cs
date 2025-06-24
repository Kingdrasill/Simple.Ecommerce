using Cache.Library.Configuration;
using Cache.Library.Core;
using Cache.Library.Management;
using ImageFile.Library.App;
using ImageFile.Library.Core.Configuration;
using ImageFile.Library.Core.Events;
using ImageFile.Library.Core.Services;
using ImageFile.Library.Infra.Cleaner;
using ImageFile.Library.Infra.Resolver;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.App.Interfaces.Services.Authentication;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CredentialService;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.App.Interfaces.Services.Dispatcher;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Entities.OrderDiscountEnity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.Entities.UserCardEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;
using Simple.Ecommerce.Domain.Settings.EmailSettings;
using Simple.Ecommerce.Domain.Settings.JwtSettings;
using Simple.Ecommerce.Domain.Settings.MongoDbSettings;
using Simple.Ecommerce.Domain.Settings.SmtpSettings;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Infra.Handlers.DeletedEvent;
using Simple.Ecommerce.Infra.Interfaces.Generic;
using Simple.Ecommerce.Infra.ReadRepositories;
using Simple.Ecommerce.Infra.Repositories;
using Simple.Ecommerce.Infra.Repositories.Generic;
using Simple.Ecommerce.Infra.Services.CacheFrequencyInitializer;
using Simple.Ecommerce.Infra.Services.Cryptography;
using Simple.Ecommerce.Infra.Services.Dispatcher;
using Simple.Ecommerce.Infra.Services.Email;
using Simple.Ecommerce.Infra.Services.JwtToken;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Simple.Ecommerce.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfra(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // SQL Database Configuration
            AddSQLDatabase(services, configuration);

            // Services Configuration (Options)
            AddConfigurations(services, configuration);

            // NoSQL Database Configuration
            AddNoSQLDatabase(services, configuration);

            // Read Model Repositories
            AddReadModelRepositories(services, configuration);

            // Image Service 
            AddImageService(services, configuration);

            // Cache Service
            AddCacheService(services, configuration);

            // Application Services
            AddApplicationServices(services, configuration);

            // Generic Repositories
            AddGenericRepositories(services, configuration);

            // List Repositories for Cache
            AddListForCacheRepositories(services, configuration);

            // Repositories
            AddRepositories(services, configuration);

            // Deleted Events
            AddDeletedEvents(services, configuration);

            return services;
        }

        private static void AddSQLDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TesteDbContext>(options =>
                options.UseMySQL(configuration.GetConnectionString("DefaultConnection")!));
        }

        private static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

            services.Configure<ImageOptions>(configuration.GetSection("ImageOptions"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<ImageOptions>>().Value);

            services.Configure<UseCache>(configuration.GetSection("UseCache"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<UseCache>>().Value);

            services.Configure<CacheOptions>(configuration.GetSection("CacheOptions"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<CacheOptions>>().Value);

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<SmtpSettings>>().Value);

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailSettings>>().Value);
        }

        private static void AddNoSQLDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(provider => 
            {
                var mongoDbSettings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(mongoDbSettings.DatabaseName);
            });
        }

        private static void AddReadModelRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IStockReadRepository, StockReadRepository>();
            services.AddSingleton<IOrderSummaryReadRepository, OrderSummaryReadRepository>();
            services.AddSingleton<IUserOrderHistoryReadRepository, UserOrderHistoryReadRepository>();
        }

        private static void AddImageService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEventDispatcher, ImageEventDispatcher>();

            services.AddSingleton<IImageStorageService>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ImageOptions>>().Value;
                var dispatcher = provider.GetRequiredService<IEventDispatcher>();
                return ServiceResolver.CreateStorage(options.Storage, options.BaseDirectory, dispatcher);
            });

            services.AddSingleton<IUsageTrackingService>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ImageOptions>>().Value;
                return ServiceResolver.CreateUsage(options.Tracking, options.MetadataFilePath, options.Capacity, options.SizeThreshold);
            });

            services.AddSingleton<IImageCompressionService>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ImageOptions>>().Value;
                return ServiceResolver.CreateCompression(options.Compression);
            });

            services.AddSingleton<IImageCleanerService>(provider =>
            {
                var storage = provider.GetRequiredService<IImageStorageService>();
                var tracking = provider.GetRequiredService<IUsageTrackingService>();
                return new ImageCleanerService(storage, tracking);
            });

            services.AddSingleton<IImageManager>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ImageOptions>>().Value;
                var storage = provider.GetRequiredService<IImageStorageService>();
                var tracking = provider.GetRequiredService<IUsageTrackingService>();
                var compression = provider.GetRequiredService<IImageCompressionService>();
                return new ImageManager(storage, tracking, compression, options.Overwrite); 
            });
        }

        private static void AddCacheService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICache>(sp => {
                var options = sp.GetRequiredService<IOptions<CacheOptions>>().Value;
                return new CacheManager(options);
            });
        }

        private static void AddApplicationServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICacheFrequencyInitializer, CacheFrequencyInitializer>();
            services.AddScoped<ICryptographyService, CryptographyService>();
            services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
            services.AddScoped<IEmailService, SmtpService>();
        }

        private static void AddGenericRepositories(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(IGenericCreateRepository<>), typeof(GenericCreateRepository<>));
            services.AddTransient(typeof(IGenericDeleteRepository<>), typeof(GenericDeleteRepository<>));
            services.AddTransient(typeof(IGenericGetRepository<>), typeof(GenericGetRepository<>));
            services.AddTransient(typeof(IGenericListRepository<>), typeof(GenericListRepository<>));
            services.AddTransient(typeof(IGenericUpdateRepository<>), typeof(GenericUpdateRepository<>));
        }

        private static void AddListForCacheRepositories(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBaseListRepository<OrderItem>, OrderItemRepository>();
            services.AddScoped<IBaseListRepository<Category>, CategoryRepository>();
            services.AddScoped<IBaseListRepository<Discount>, DiscountRepository>();
            services.AddScoped<IBaseListRepository<DiscountBundleItem>, DiscountBundleItemRepository>();
            services.AddScoped<IBaseListRepository<DiscountTier>, DiscountTierRepository>();
            services.AddScoped<IBaseListRepository<Login>, LoginRepository>();
            services.AddScoped<IBaseListRepository<Order>, OrderRepository>();
            services.AddScoped<IBaseListRepository<OrderDiscount>, OrderDiscountRepository>();
            services.AddScoped<IBaseListRepository<Product>, ProductRepository>();
            services.AddScoped<IBaseListRepository<ProductCategory>, ProductCategoryRepository>();
            services.AddScoped<IBaseListRepository<ProductDiscount>, ProductDiscountRepository>();
            services.AddScoped<IBaseListRepository<ProductPhoto>, ProductPhotoRepository>();
            services.AddScoped<IBaseListRepository<Review>, ReviewRepository>();
            services.AddScoped<IBaseListRepository<User>, UserRepository>();
            services.AddScoped<IBaseListRepository<UserAddress>, UserAddressRepository>();
            services.AddScoped<IBaseListRepository<UserCard>, UserCardRepository>();
        }

        private static void AddRepositories(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICacheFrequencyRepository, CacheFrequencyRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICredentialVerificationRepository, CredentialVerificationRepository>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<IDiscountBundleItemRepository, DiscountBundleItemRepository>();
            services.AddScoped<IDiscountTierRepository, DiscountTierRepository>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDiscountRepository, OrderDiscountRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IProductDiscountRepository, ProductDiscountRepository>();
            services.AddScoped<IProductPhotoRepository, ProductPhotoRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserAddressRepository, UserAddressRepository>();
            services.AddScoped<IUserCardRepository, UserCardRepository>();
        }

        private static void AddDeletedEvents(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDeleteEventDispatcher, DeleteEventDispatcher>();
            services.AddScoped<IDeleteEventHandler<CacheFrequencyDeletedEvent>, CacheFrequencyDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<CartItemDeletedEvent>, OrderItemDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<CategoryDeletedEvent>, CategoryDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<CouponDeletedEvent>, CouponDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<CredentialVerificationDeletedEvent>, CredentialVerificationDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<DiscountDeletedEvent>, DiscountDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<DiscountBundleItemDeletedEvent>, DiscountBundleItemDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<DiscountTierDeletedEvent>, DiscountTierDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<LoginDeletedEvent>, LoginDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<OrderDeletedEvent>, OrderDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<OrderDiscountDeletedEvent>, OrderDiscountDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<ProductDeletedEvent>, ProductDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<ProductCategoryDeletedEvent>, ProductCategoryDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<ProductDiscountDeletedEvent>, ProductDiscountDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<ProductPhotoDeletedEvent>, ProductPhotoDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<ReviewDeletedEvent>, ReviewDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<UserDeletedEvent>, UserDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<UserAddressDeletedEvent>, UserAddressDeletedEventHandler>();
            services.AddScoped<IDeleteEventHandler<UserCardDeletedEvent>, UserCardDeletedEventHandler>();
        }
    }
}
