using Simple.Ecommerce.App.Interfaces.Services.Dispatcher;
using Simple.Ecommerce.Domain.Entities;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Entities.FrequencyEntity;
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
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;

namespace Simple.Ecommerce.Infra
{
    public class TesteDbContext : DbContext
    {
        private readonly IDeleteEventDispatcher _dispatcher;

        public TesteDbContext(
            DbContextOptions<TesteDbContext> options,
            IDeleteEventDispatcher dispatcher
        ) : base(options) 
        { 
            _dispatcher = dispatcher;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TesteDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = ChangeTracker.Entries<BaseEntity>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            await _dispatcher.Dispatch(domainEvents);

            if (ChangeTracker.HasChanges())
            {
                await base.SaveChangesAsync(cancellationToken);
            }

            foreach (var entity in ChangeTracker.Entries<BaseEntity>())
            {
                entity.Entity.ClearDomainEvents();
            }

            return result;
        }

        public DbSet<CacheFrequency> CacheFrequencies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CredentialVerification> CredentialVerifications { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountBundleItem> DiscountBundleItems { get; set; }
        public DbSet<DiscountTier> DiscountTiers { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDiscount> OrderDiscounts { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductDiscount> ProductDiscounts { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
    }
}
