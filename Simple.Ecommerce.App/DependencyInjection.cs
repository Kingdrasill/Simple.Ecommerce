using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simple.Ecommerce.App.Interfaces.Commands.CategoryCommands;
using Simple.Ecommerce.App.Interfaces.Commands.CredentialVerificationCommands;
using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Commands.LoginCommands;
using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Commands.ReviewCommands;
using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Queries.CacheFrequencyQueries;
using Simple.Ecommerce.App.Interfaces.Queries.CategoryQueries;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.App.Interfaces.Queries.OrderItemQueries;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.App.Interfaces.Queries.ProductQueries;
using Simple.Ecommerce.App.Interfaces.Queries.ReviewQueries;
using Simple.Ecommerce.App.Interfaces.Queries.UserQueries;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.CardService;
using Simple.Ecommerce.App.Interfaces.Services.ImageCleanup;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Interfaces.Services.ServiceResolver;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.App.Services.CardService;
using Simple.Ecommerce.App.Services.FileImage;
using Simple.Ecommerce.App.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.ServiceResolver;
using Simple.Ecommerce.App.UseCases.CacheFrequencyCases.Queries;
using Simple.Ecommerce.App.UseCases.CategoryCases.Commands;
using Simple.Ecommerce.App.UseCases.CategoryCases.Queries;
using Simple.Ecommerce.App.UseCases.CredentialVerificationCases.Commands;
using Simple.Ecommerce.App.UseCases.DiscountCases.Commands;
using Simple.Ecommerce.App.UseCases.DiscountCases.Queries;
using Simple.Ecommerce.App.UseCases.LoginCases.Commands;
using Simple.Ecommerce.App.UseCases.OrderCases.Commands;
using Simple.Ecommerce.App.UseCases.OrderCases.Queries;
using Simple.Ecommerce.App.UseCases.OrderItemCases.Commands;
using Simple.Ecommerce.App.UseCases.OrderItemCases.Queries;
using Simple.Ecommerce.App.UseCases.ProductCases.Commands;
using Simple.Ecommerce.App.UseCases.ProductCases.Queries;
using Simple.Ecommerce.App.UseCases.ReviewCases.Commands;
using Simple.Ecommerce.App.UseCases.ReviewCases.Queries;
using Simple.Ecommerce.App.UseCases.UserCases.Commands;
using Simple.Ecommerce.App.UseCases.UserCases.Queries;

namespace Simple.Ecommerce.App
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApp(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Service Repository Resolver
            services.AddScoped<IRepositoryResolver, RepositoryResolver>();

            // Cache Handler
            services.AddScoped<ICacheHandler, CacheHandler>();

            // Card Service
            services.AddScoped<ICardService, CardService>();

            // Repository Handler
            services.AddScoped<IRepositoryHandler, RepositotyHandler>();

            // CacheFrequency Batch
            services.AddScoped<IListCacheFrequencyQuery, ListCacheFrequencyQuery>();

            // Image Cleanup Entities
            services.AddScoped<IImageCleanup, ProductPhotoImageCleanup>();
            services.AddScoped<IImageCleanup, UserImageCleanup>();

            // Category Batch
            services.AddScoped<ICreateCategoryCommand, CreateCategoryCommand>();
            services.AddScoped<IDeleteCategoryCommand, DeleteCategoryCommand>();
            services.AddScoped<IUpdateCategoryCommand, UpdateCategoryCommand>();
            services.AddScoped<IGetCategoryQuery, GetCategoryQuery>();
            services.AddScoped<IListCategoryQuery, ListCategoryQuery>();

            // CredentialVerification Batch
            services.AddScoped<ICreateCredentialVerificationCommand, CreateCredentialVerficationCommand>();
            services.AddScoped<IConfirmCredentialVerificationCommand, ConfirmCredentialVerificationCommand>();

            // Discount Batch
            services.AddScoped<ICreateDiscountCommand, CreateDiscountCommand>();
            services.AddScoped<ICreateBatchCouponsDiscountCommand, CreateBatchCouponsDiscountCommand>();
            services.AddScoped<ICreateDiscountBundleItemDiscountCommand, CreateDiscountBundleItemDiscountCommand>();
            services.AddScoped<ICreateDiscountTierDiscountCommand, CreateDiscountTierDiscountCommand>();
            services.AddScoped<IDeleteDiscountCommand, DeleteDiscountCommand>();
            services.AddScoped<IDeleteCouponDiscountCommand, DeleteCouponDiscountCommand>();
            services.AddScoped<IDeleteDiscountBundleItemDiscountCommand, DeleteDiscountBundleItemDiscountCommand>();
            services.AddScoped<IDeleteDiscountTierDiscountCommand, DeleteDiscountTierDiscountCommand>();
            services.AddScoped<IUpdateDiscountCommand, UpdateDiscountCommand>();
            services.AddScoped<IUpdateCouponDiscountCommand, UpdateCouponDiscountCommand>();
            services.AddScoped<IUpdateDiscountBundleItemDiscountCommand, UpdateDiscountBundleItemDiscountCommand>();
            services.AddScoped<IUpdateDiscountTierDiscountCommand, UpdateDiscountTierDiscountCommand>();
            services.AddScoped<IUseCouponDiscountCommand, UseCouponDiscountCommand>();
            services.AddScoped<IGetDiscountQuery, GetDiscountQuery>();
            services.AddScoped<IGetDiscountDTOQuery, GetDiscountDTOQuery>();
            services.AddScoped<IGetCouponDiscountQuery, GetCouponDiscountQuery>();
            services.AddScoped<IGetDiscountBundleItemDiscountQuery, GetDiscountBundleItemDiscountQuery>();
            services.AddScoped<IGetDiscountTierDiscountQuery, GetDiscountTierDiscountQuery>();
            services.AddScoped<IListDiscountQuery, ListDiscountQuery>();
            services.AddScoped<IListDiscountDTOQuery, ListDiscountDTOQuery>();
            services.AddScoped<IListCouponDiscountQuery, ListCouponDiscountQuery>();
            services.AddScoped<IListDiscountBundleItemDiscountQuery, ListDiscountBundleItemDiscountQuery>();
            services.AddScoped<IListDiscountTierDiscountQuery, ListDiscountTierDiscountQuery>();

            // Login Batch
            services.AddScoped<ICreateLoginCommand, CreateLoginCommand>();
            services.AddScoped<IDeleteLoginCommand, DeleteLoginCommand>();
            services.AddScoped<IAuthenticateLoginCommand, AuthenticateLoginCommand>();

            // Order Batch
            services.AddScoped<IAddDiscountOrderCommand, AddDiscountOrderCommand>();
            services.AddScoped<ICancelOrderCommand, CancelOrderCommand>();
            services.AddScoped<IConfirmOrderCommand, ConfirmOrderCommand>();
            services.AddScoped<IChangePaymentMethodOrderCommand, ChangePaymentMethodOrderCommand>();
            services.AddScoped<ICreateOrderCommand, CreateOrderCommand>();
            services.AddScoped<IDeleteDiscountOrderCommand, DeleteDiscountOrderCommand>();
            services.AddScoped<IDeleteOrderCommand, DeleteOrderCommand>();
            services.AddScoped<IRemovePaymentMethodOrderCommand, RemovePaymentMethodOrderCommand>();
            services.AddScoped<IUpdateOrderCommand, UpdateOrderCommand>();
            services.AddScoped<IGetDiscountDTOsOrderQuery, GetDiscountDTOsOrderQuery>();
            services.AddScoped<IGetPaymentMethodOrderQuery, GetPaymentMethodOrderQuery>();
            services.AddScoped<IGetOrderQuery, GetOrderQuery>();
            services.AddScoped<IListOrderQuery, ListOrderQuery>();

            // OrderItem Batch
            services.AddScoped<IAddItemsOrderItemCommand, AddItemsOrderItemCommand>();
            services.AddScoped<IGetOrderItemQuery, GetOrderItemQuery>();
            services.AddScoped<IListOrderItemQuery, ListOrderItemQuery>();

            // Product Batch
            services.AddScoped<IAddCategoryProductCommand, AddCategoryProductCommand>();
            services.AddScoped<IAddDiscountProductCommand, AddDiscountProductCommand>();
            services.AddScoped<IAddPhotoProductCommand, AddPhotoProductCommand>();
            services.AddScoped<ICreateProductCommand, CreateProductCommand>();
            services.AddScoped<IDeleteCategoryProductCommand, DeleteCategoryProductCommand>();
            services.AddScoped<IDeleteDiscountProductCommand, DeleteDiscountProductCommand>();
            services.AddScoped<IDeletePhotoProductCommand, DeletePhotoProductCommand>();
            services.AddScoped<IDeleteProductCommand, DeleteProductCommand>();
            services.AddScoped<IUpdateProductCommand, UpdateProductCommand>();
            services.AddScoped<IGetCategoriesProductQuery, GetCategoriesProductQuery>();
            services.AddScoped<IGetDiscountsProductQuery, GetDiscountsProductQuery>();
            services.AddScoped<IGetPhotosProductQuery, GetPhotosProductQuery>();
            services.AddScoped<IGetProductQuery, GetProductQuery>();
            services.AddScoped<IListProductQuery, ListProductQuery>();

            // Review Batch
            services.AddScoped<ICreateReviewCommand, CreateReviewCommand>();
            services.AddScoped<IDeleteReviewCommand, DeleteReviewCommand>();
            services.AddScoped<IUpdateReviewCommand, UpdateReviewCommand>();
            services.AddScoped<IGetReviewQuery, GetReviewQuery>();
            services.AddScoped<IListReviewQuery, ListReviewQuery>();

            // User Batch
            services.AddScoped<IAddAddressUserCommand, AddAddressUserCommand>();
            services.AddScoped<IAddCardUserCommand, AddCardUserCommand>();
            services.AddScoped<IAddPhotoUserCommand, AddPhotoUserCommand>();
            services.AddScoped<ICreateUserCommand, CreateUserCommand>();
            services.AddScoped<IDeleteUserCommand, DeleteUserCommand>();
            services.AddScoped<IRemoveAddressUserCommand, RemoveAddressUserCommand>();
            services.AddScoped<IRemoveCardUserCommand, RemoveCardUserCommand>();
            services.AddScoped<IRemovePhotoUserCommand, RemovePhotoUserCommand>();
            services.AddScoped<IUpdateUserCommand, UpdateUserCommand>();
            services.AddScoped<IGetAddressesUserQuery, GetAddressesUserQuery>();
            services.AddScoped<IGetCardsUserQuery, GetCardsUserQuery>();
            services.AddScoped<IGetPhotoUserQuery, GetPhotoUserQuery>();
            services.AddScoped<IGetUserQuery, GetUserQuery>();
            services.AddScoped<IListUserQuery, ListUserQuery>();

            return services;
        }
    }
}
