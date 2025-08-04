using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO;
using Simple.Ecommerce.Contracts.OrderContracts.Discounts;
using Simple.Ecommerce.Contracts.OrderContracts.PaymentInformations;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ICreateOrderCommand _createOrderCommand;
        private readonly IDeleteOrderCommand _deleteOrderCommand;
        private readonly IUpdateOrderCommand _updateOrderCommand;
        private readonly IGetOrderQuery _getOrderQuery;
        private readonly IListOrderQuery _listOrderQuery;
        private readonly ICancelOrderCommand _cancelOrderCommand;
        private readonly IConfirmOrderCommand _confirmOrderCommand;
        private readonly IConfirmNewOrderCommand _confirmNewOrderCommand;
        private readonly IChangeDiscountOrderCommand _changeDiscountOrderCommand;
        private readonly IChangePaymentInformationOrderCommand _changePaymentMethodOrderCommand;
        private readonly IRemovePaymentMethodOrderCommand _removePaymentMethodOrderCommand;
        private readonly IRevertProcessedOrderCommand _revertProcessedOrderCommand;
        private readonly IGetCompleteOrderQuery _getCompleteOrderQuery;
        private readonly IGetPaymentInformationOrderQuery _getPaymentMethodOrderQuery;

        public OrderController(
            ICreateOrderCommand createOrderCommand,
            IDeleteOrderCommand deleteOrderCommand,
            IUpdateOrderCommand updateOrderCommand,
            IGetOrderQuery getOrderQuery,
            IListOrderQuery listOrderQuery,
            ICancelOrderCommand cancelOrderCommand,
            IConfirmOrderCommand confirmOrderCommand,
            IConfirmNewOrderCommand confirmNewOrderCommand,
            IChangeDiscountOrderCommand changeDiscountOrderCommand,
            IChangePaymentInformationOrderCommand changePaymentMethodOrderCommand,
            IRemovePaymentMethodOrderCommand removePaymentMethodOrderCommand,
            IRevertProcessedOrderCommand revertProcessedOrderCommand,
            IGetCompleteOrderQuery getCompleteOrderQuery,
            IGetPaymentInformationOrderQuery getPaymentMethodOrderQuery
        )
        {
            _createOrderCommand = createOrderCommand;
            _deleteOrderCommand = deleteOrderCommand;
            _updateOrderCommand = updateOrderCommand;
            _getOrderQuery = getOrderQuery;
            _listOrderQuery = listOrderQuery;
            _cancelOrderCommand = cancelOrderCommand;
            _confirmOrderCommand = confirmOrderCommand;
            _confirmNewOrderCommand = confirmNewOrderCommand;
            _changeDiscountOrderCommand = changeDiscountOrderCommand;
            _changePaymentMethodOrderCommand = changePaymentMethodOrderCommand;
            _removePaymentMethodOrderCommand = removePaymentMethodOrderCommand;
            _revertProcessedOrderCommand = revertProcessedOrderCommand;
            _getCompleteOrderQuery = getCompleteOrderQuery;
            _getPaymentMethodOrderQuery = getPaymentMethodOrderQuery;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderResponse>> Post([FromBody] OrderRequest request)
        {
            if (request.CouponCode is not null && request.DiscountId is not null)
            {
                return BadRequest($"OrderController.Conflict.Discount: Não se pode passar um código para coupon ({request.CouponCode}) e um desconto ({request.DiscountId}) ao mesmo tempo para um pedido!\n");
            }

            var result = await _createOrderCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _deleteOrderCommand.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<OrderResponse>> Put([FromBody] OrderRequest request)
        {
            if (request.CouponCode is not null && request.DiscountId is not null)
            {
                return BadRequest($"OrderController.Conflict.Discount: Não se pode passar um código para coupon ({request.CouponCode}) e um desconto ({request.DiscountId}) ao mesmo tempo para um pedido!\n");
            }

            var result = await _updateOrderCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Cancel/{orderId}")]
        [Authorize]
        public async Task<ActionResult<bool>> Cancel(int orderId)
        {
            var result = await _cancelOrderCommand.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Confirm")]
        public async Task<ActionResult<bool>> Confirm([FromBody] OrderCompleteRequest request)
        {
            if (request.CouponCode is not null && request.DiscountId is not null)
            {
                return BadRequest($"OrderController.Conflict.Discount: Não se pode passar um código para coupon ({request.CouponCode}) e um desconto ({request.DiscountId}) ao mesmo tempo para um pedido!\n");
            }

            foreach (var item in request.OrderItems)
            {
                string message = "";
                if (item.CouponCode is not null && item.ProductDiscountId is not null)
                {
                    message += $"Não se pode passar um código para coupon ({item.CouponCode}) e um desconto para produto ({item.ProductDiscountId}) ao mesmo tempo para o produto {item.ProductId}!\n";
                }
                if (!message.Equals(""))
                    return BadRequest(message);
            }

            var result = await _confirmNewOrderCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Confim/{orderId}")]
        [Authorize]
        public async Task<ActionResult<bool>> Confirm(int orderId)
        {
            var result = await _confirmOrderCommand.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Revert/{orderId}")]
        [Authorize]
        public async Task<ActionResult<bool>> Revert(int orderId)
        {
            var result = await _revertProcessedOrderCommand.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Discount/Change")]
        [Authorize]
        public async Task<ActionResult<bool>> ChangeDiscount([FromBody] OrderDiscountRequest request)
        {
            if (request.CouponCode is not null && request.DiscountId is not null)
            {
                return BadRequest($"OrderController.Conflict.Discount: Não se pode passar um código para coupon ({request.CouponCode}) e um desconto ({request.DiscountId}) ao mesmo tempo para um pedido!\n");
            }

            var result = await _changeDiscountOrderCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("PaymentMethod/Change")]
        [Authorize]
        public async Task<ActionResult<OrderPaymentInformationResponse>> ChangePaymentMethod([FromBody] OrderPaymentInformationRequest request)
        {
            var result = await _changePaymentMethodOrderCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("PaymentMethod/Remove/{orderId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemovePaymentMethod(int orderId)
        {
            var result = await _removePaymentMethodOrderCommand.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderResponse>> Get(int id)
        {
            var result = await _getOrderQuery.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<OrderResponse>>> List()
        {
            var result = await _listOrderQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("ConfirmedOrder/{orderId}")]
        [Authorize]
        public async Task<ActionResult<OrderPaymentInformationResponse>> GetCompleteOrder(int orderId)
        {
            var result = await _getCompleteOrderQuery.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("PaymentMethod/{orderId}")]
        [Authorize]
        public async Task<ActionResult<OrderPaymentInformationResponse>> GetPaymentMethod(int orderId)
        {
            var result = await _getPaymentMethodOrderQuery.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }
    }
}
