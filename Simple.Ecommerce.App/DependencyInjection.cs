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
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.App.Interfaces.Services.ServiceResolver;
using Simple.Ecommerce.App.Services.Cache;
using Simple.Ecommerce.App.Services.CardService;
using Simple.Ecommerce.App.Services.FileImage;
using Simple.Ecommerce.App.Services.OrderProcessing.ChainOfResponsibility;
using Simple.Ecommerce.App.Services.OrderProcessing.Dispatcher;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.CouponsValidationHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.DiscountsValidationHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsBOGOHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsBundleHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsSimpleHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsTieredHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.OrderDiscountHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ShippingHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.StockValidationHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.TaxHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ItemsBOGOHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ItemsBundleHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ItemsSimpleHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ItemsTieredHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.OrderDiscountHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ShippingHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.StockValidationHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.TaxHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Processor;
using Simple.Ecommerce.App.Services.OrderProcessing.Projectors;
using Simple.Ecommerce.App.Services.RepositoryHandler;
using Simple.Ecommerce.App.Services.ServiceResolver;
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
using Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderProcessEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderRevertEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.StockEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent;

namespace Simple.Ecommerce.App
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApp(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Cache handling helpers
            AddCacheHelpers(services, configuration);

            // Card Service
            services.AddScoped<ICardService, CardService>();

            // Repository Handler
            services.AddScoped<IRepositoryHandler, RepositotyHandler>();

            // Order Processing
            AddOrderProcessing(services, configuration);

            // Image Cleanup
            AddImageCleanups(services, configuration);

            // Category Batch
            AddCategoryBatch(services, configuration);

            // CredentialVerification Batch
            AddCredentialVerificationBatch(services, configuration);

            // Discount Batch
            AddDiscountBatch(services, configuration);

            // Login Batch
            AddLoginBatch(services, configuration);

            // Order Batch
            AddOrderBatch(services, configuration);

            // OrderItem Batch
            AddOrderItemBatch(services, configuration);

            // Product Batch
            AddProductBatch(services, configuration);

            // Review Batch
            AddReviewBatch(services, configuration);

            // User Batch
            AddUserBatch(services, configuration);

            return services;
        }

        private static void AddCacheHelpers(this IServiceCollection services, IConfiguration configuration)
        {
            // Service Repository Resolver
            services.AddScoped<IRepositoryResolver, RepositoryResolver>();

            // Cache Handler
            services.AddScoped<ICacheHandler, CacheHandler>();
        }

        private static void AddOrderProcessing(this IServiceCollection services, IConfiguration configuration)
        {
            // Order Processing Event Dispatcher
            services.AddScoped<IOrderProcessingDispatcher, OrderProcessingDispatcher>();

            // Order Details Projector
            AddOrderDetailProjector(services, configuration);

            // Order Event Stream Projector
            AddOrderEventStreamProjetor(services, configuration);

            // Order Summary Projector
            AddOrderSummaryProjector(services, configuration);

            // Stock Movement Projector
            AddStockMovementProjector(services, configuration);

            // User Order History Projector
            AddUserOrderHistoryProjector(services, configuration);

            // Chain Of Responsibility Handlers
            AddCoRHandlers(services, configuration);

            // Chain of Responsibilities
            services.AddScoped<IOrderProcessingChain, ConfirmOrderProcessingChain>();

            // Revert Handlers
            AddRevertHandlers(services, configuration);

            // Revert Handlers Processor
            services.AddScoped<OrderRevertProcessor>();

            // Process Confirmed Order Command Handler
            services.AddScoped<ProcessConfirmedOrderCommandHandler>();
            services.AddScoped<ProcessConfirmedNewOrderCommandHandler>();
            services.AddScoped<RevertOrderCommandHandler>();
        }

        private static void AddOrderDetailProjector(this IServiceCollection services, IConfiguration configuration)
        {
            // Confirmação
            services.AddScoped<IOrderProcessingEventHandler<OrderProcessingStartedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderStatusChangedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<ShippingFeeAppliedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<SimpleItemDiscountAppliedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TieredItemDiscountAppliedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BOGODiscountAppliedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BundleDiscountAppliedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderDiscountAppliedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TaxAppliedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderProcessedEvent>, OrderDetailProjector>();
            // Reversão
            services.AddScoped<IOrderProcessingEventHandler<OrderRevertingStartedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TaxRevertedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderDiscountRevertedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BundleDiscountRevertEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BOGODiscountRevertEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TieredItemDiscountRevertEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<SimpleItemDiscountRevertEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<ShippingFeeRevertedEvent>, OrderDetailProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderRevertedEvent>, OrderDetailProjector>();
        }

        private static void AddOrderEventStreamProjetor(this IServiceCollection services, IConfiguration configuration)
        {
            // Confirmação
            services.AddScoped<IOrderProcessingEventHandler<OrderProcessingStartedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderStatusChangedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<ShippingFeeAppliedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<SimpleItemDiscountAppliedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TieredItemDiscountAppliedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BOGODiscountAppliedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BundleDiscountAppliedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderDiscountAppliedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TaxAppliedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderProcessedEvent>, OrderEventStreamProjector>();
            // Reversão
            services.AddScoped<IOrderProcessingEventHandler<OrderRevertingStartedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TaxRevertedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderDiscountRevertedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BundleDiscountRevertEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BOGODiscountRevertEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TieredItemDiscountRevertEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<SimpleItemDiscountRevertEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<ShippingFeeRevertedEvent>, OrderEventStreamProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderRevertedEvent>, OrderEventStreamProjector>();
        }

        private static void AddOrderSummaryProjector(this IServiceCollection services, IConfiguration configuration)
        {
            // Confirmação
            services.AddScoped<IOrderProcessingEventHandler<OrderProcessingStartedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderStatusChangedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<ShippingFeeAppliedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<SimpleItemDiscountAppliedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TieredItemDiscountAppliedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BOGODiscountAppliedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BundleDiscountAppliedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderDiscountAppliedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TaxAppliedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderProcessedEvent>, OrderSummaryProjector>();
            // Reversão
            services.AddScoped<IOrderProcessingEventHandler<OrderRevertingStartedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TaxRevertedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderDiscountRevertedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BundleDiscountRevertEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BOGODiscountRevertEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TieredItemDiscountRevertEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<SimpleItemDiscountRevertEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<ShippingFeeRevertedEvent>, OrderSummaryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderRevertedEvent>, OrderSummaryProjector>();
        }

        private static void AddStockMovementProjector(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderProcessingEventHandler<StockReservedEvent>, StockMovementProjector>();
            services.AddScoped<IOrderProcessingEventHandler<StockReleasedEvent>, StockMovementProjector>();
        }

        private static void AddUserOrderHistoryProjector(this IServiceCollection services, IConfiguration configuration)
        {
            // Confirmação
            services.AddScoped<IOrderProcessingEventHandler<OrderProcessingStartedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderStatusChangedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<ShippingFeeAppliedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<SimpleItemDiscountAppliedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TieredItemDiscountAppliedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BOGODiscountAppliedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BundleDiscountAppliedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderDiscountAppliedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TaxAppliedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderProcessedEvent>, UserOrderHistoryProjector>();
            // Reversão
            services.AddScoped<IOrderProcessingEventHandler<OrderRevertingStartedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TaxRevertedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderDiscountRevertedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BundleDiscountRevertEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<BOGODiscountRevertEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<TieredItemDiscountRevertEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<SimpleItemDiscountRevertEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<ShippingFeeRevertedEvent>, UserOrderHistoryProjector>();
            services.AddScoped<IOrderProcessingEventHandler<OrderRevertedEvent>, UserOrderHistoryProjector>();
        }

        private static void AddCoRHandlers(this IServiceCollection services, IConfiguration configuration)
        {
            // Confirmação
            services.AddScoped<CouponsValidationHandler>();
            services.AddScoped<StockValidationHandler>();
            services.AddScoped<ShippingHandler>();
            services.AddScoped<DiscountsValidationHandler>();
            services.AddScoped<SimpleItemsDiscountHandler>();
            services.AddScoped<TieredItemsDiscountHandler>();
            services.AddScoped<BOGOItemsDiscountHandler>();
            services.AddScoped<BundleItemsDiscountHandler>();
            services.AddScoped<OrderDiscountHandler>();
            services.AddScoped<TaxHandler>();
        }

        private static void AddRevertHandlers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderRevertingHandler, RevertTaxHandler>();
            services.AddScoped<IOrderRevertingHandler, ReverOrderDiscountHandler>();
            services.AddScoped<IOrderRevertingHandler, RevertBundleItemDiscountHandler>();
            services.AddScoped<IOrderRevertingHandler, RevertBOGOItemDiscountHandler>();
            services.AddScoped<IOrderRevertingHandler, RevertTieredItemDiscountHandler>();
            services.AddScoped<IOrderRevertingHandler, RevertSimpleItemDiscountHandler>();
            services.AddScoped<IOrderRevertingHandler, RevertShippingHandler>();
            services.AddScoped<IOrderRevertingHandler, RevertStockValidationHandler>();
        }

        private static void AddImageCleanups(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IImageCleanup, ProductPhotoImageCleanup>();
            services.AddScoped<IImageCleanup, UserImageCleanup>();
        }

        private static void AddCategoryBatch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICreateCategoryCommand, CreateCategoryCommand>();
            services.AddScoped<IDeleteCategoryCommand, DeleteCategoryCommand>();
            services.AddScoped<IUpdateCategoryCommand, UpdateCategoryCommand>();
            services.AddScoped<IGetCategoryQuery, GetCategoryQuery>();
            services.AddScoped<IListCategoryQuery, ListCategoryQuery>();
        }

        private static void AddCredentialVerificationBatch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICreateCredentialVerificationCommand, CreateCredentialVerficationCommand>();
            services.AddScoped<IConfirmCredentialVerificationCommand, ConfirmCredentialVerificationCommand>();
        }

        private static void AddDiscountBatch(this IServiceCollection services, IConfiguration configuration)
        {
            // Discount 
            services.AddScoped<IToggleActivationDiscountCommand, ToggleActivationDiscountCommand>();
            services.AddScoped<IGetDiscountDTOQuery, GetDiscountDTOQuery>();
            services.AddScoped<IListDiscountDTOQuery, ListDiscountDTOQuery>();
            // CRUD
            services.AddScoped<ICreateDiscountCommand, CreateDiscountCommand>();
            services.AddScoped<IDeleteDiscountCommand, DeleteDiscountCommand>();
            services.AddScoped<IUpdateDiscountCommand, UpdateDiscountCommand>();
            services.AddScoped<IGetDiscountQuery, GetDiscountQuery>();
            services.AddScoped<IListDiscountQuery, ListDiscountQuery>();

            // Coupon Batch
            services.AddScoped<ICreateBatchCouponsDiscountCommand, CreateBatchCouponsDiscountCommand>();
            // CRUD
            services.AddScoped<IDeleteCouponDiscountCommand, DeleteCouponDiscountCommand>();
            services.AddScoped<IUpdateCouponDiscountCommand, UpdateCouponDiscountCommand>();
            services.AddScoped<IGetCouponDiscountQuery, GetCouponDiscountQuery>();
            services.AddScoped<IListCouponDiscountQuery, ListCouponDiscountQuery>();

            // DiscountBundleItem Batch
            // CRUD
            services.AddScoped<ICreateBundleItemDiscountCommand, CreateBundleItemDiscountCommand>();
            services.AddScoped<IDeleteBundleItemDiscountCommand, DeleteBundleItemDiscountCommand>();
            services.AddScoped<IUpdateBundleItemDiscountCommand, UpdateBundleItemDiscountCommand>();
            services.AddScoped<IGetBundleItemDiscountQuery, GetBundleItemDiscountQuery>();
            services.AddScoped<IListBundleItemDiscountQuery, ListBundleItemDiscountQuery>();

            // DiscountTier Batch
            // CRUD
            services.AddScoped<ICreateTierDiscountCommand, CreateTierDiscountCommand>();
            services.AddScoped<IDeleteTierDiscountCommand, DeleteTierDiscountCommand>();
            services.AddScoped<IUpdateTierDiscountCommand, UpdateTierDiscountCommand>();
            services.AddScoped<IGetTierDiscountQuery, GetTierDiscountQuery>();
            services.AddScoped<IListTierDiscountQuery, ListTierDiscountQuery>();
        }

        private static void AddLoginBatch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICreateLoginCommand, CreateLoginCommand>();
            services.AddScoped<IDeleteLoginCommand, DeleteLoginCommand>();
            services.AddScoped<IAuthenticateLoginCommand, AuthenticateLoginCommand>();
        }

        private static void AddOrderBatch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICancelOrderCommand, CancelOrderCommand>();
            services.AddScoped<IChangeDiscountOrderCommand, ChangeDiscountOrderCommand>();
            services.AddScoped<IChangePaymentInformationOrderCommand, ChangePaymentInformationOrderCommand>();
            services.AddScoped<IConfirmOrderCommand, ConfirmOrderCommand>();
            services.AddScoped<IConfirmNewOrderCommand, ConfirmNewOrderCommand>();
            services.AddScoped<IRemovePaymentMethodOrderCommand, RemovePaymentMethodOrderCommand>();
            services.AddScoped<IRevertProcessedOrderCommand, RevertProcessedOrderCommand>();
            services.AddScoped<IGetCompleteOrderQuery, GetCompleteOrderQuery>();
            services.AddScoped<IGetPaymentInformationOrderQuery, GetPaymentMethodOrderQuery>();
            //CRUD
            services.AddScoped<ICreateOrderCommand, CreateOrderCommand>();
            services.AddScoped<IDeleteOrderCommand, DeleteOrderCommand>();
            services.AddScoped<IUpdateOrderCommand, UpdateOrderCommand>();
            services.AddScoped<IGetOrderQuery, GetOrderQuery>();
            services.AddScoped<IListOrderQuery, ListOrderQuery>();
        }

        private static void AddOrderItemBatch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAddItemOrderItemCommand, AddItemOrderItemCommand>();
            services.AddScoped<IAddItemsOrderItemCommand, AddItemsOrderItemCommand>();
            services.AddScoped<IChangeDiscountOrderItemCommand, ChangeDiscountOrderItemCommand>();
            services.AddScoped<IRemoveItemOrderItemCommand, RemoveItemOrderItemCommand>();
            services.AddScoped<IRemoveAllItemsOrderItemCommand, RemoveAllItemsOrderItemCommand>();
            // CRUD
            services.AddScoped<IGetOrderItemQuery, GetOrderItemQuery>();
            services.AddScoped<IListOrderItemQuery, ListOrderItemQuery>();
        }

        private static void AddProductBatch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAddCategoryProductCommand, AddCategoryProductCommand>();
            services.AddScoped<IAddDiscountProductCommand, AddDiscountProductCommand>();
            services.AddScoped<IAddPhotoProductCommand, AddPhotoProductCommand>();
            services.AddScoped<IRemoveCategoryProductCommand, RemoveCategoryProductCommand>();
            services.AddScoped<IRemoveDiscountProductCommand, RemoveDiscountProductCommand>();
            services.AddScoped<IRemovePhotoProductCommand, RemovePhotoProductCommand>();
            services.AddScoped<IGetCategoriesProductQuery, GetCategoriesProductQuery>();
            services.AddScoped<IGetDiscountsProductQuery, GetDiscountsProductQuery>();
            services.AddScoped<IGetPhotosProductQuery, GetPhotosProductQuery>();
            // CRUD
            services.AddScoped<ICreateProductCommand, CreateProductCommand>();
            services.AddScoped<IDeleteProductCommand, DeleteProductCommand>();
            services.AddScoped<IUpdateProductCommand, UpdateProductCommand>();
            services.AddScoped<IGetProductQuery, GetProductQuery>();
            services.AddScoped<IListProductQuery, ListProductQuery>();
        }

        private static void AddReviewBatch(this IServiceCollection services, IConfiguration configuration)
        {
            // CRUD
            services.AddScoped<ICreateReviewCommand, CreateReviewCommand>();
            services.AddScoped<IDeleteReviewCommand, DeleteReviewCommand>();
            services.AddScoped<IUpdateReviewCommand, UpdateReviewCommand>();
            services.AddScoped<IGetReviewQuery, GetReviewQuery>();
            services.AddScoped<IListReviewQuery, ListReviewQuery>();
        }

        private static void AddUserBatch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAddAddressUserCommand, AddAddressUserCommand>();
            services.AddScoped<IAddPaymentUserCommand, AddPaymentUserCommand>();
            services.AddScoped<IAddPhotoUserCommand, AddPhotoUserCommand>();
            services.AddScoped<IRemoveAddressUserCommand, RemoveAddressUserCommand>();
            services.AddScoped<IRemovePaymentUserCommand, RemovePaymentUserCommand>();
            services.AddScoped<IRemovePhotoUserCommand, RemovePhotoUserCommand>();
            services.AddScoped<IGetAddressesUserQuery, GetAddressesUserQuery>();
            services.AddScoped<IGetPaymentsUserQuery, GetPaymentsUserQuery>();
            services.AddScoped<IGetPhotoUserQuery, GetPhotoUserQuery>();
            // CRUD
            services.AddScoped<ICreateUserCommand, CreateUserCommand>();
            services.AddScoped<IDeleteUserCommand, DeleteUserCommand>();
            services.AddScoped<IUpdateUserCommand, UpdateUserCommand>();
            services.AddScoped<IGetUserQuery, GetUserQuery>();
            services.AddScoped<IListUserQuery, ListUserQuery>();
        }
    }
}
