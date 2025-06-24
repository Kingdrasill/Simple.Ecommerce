using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Contracts.OrderDiscountContracts;

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
        private readonly IAddDiscountOrderCommand _addDiscountOrderCommand;
        private readonly ICancelOrderCommand _cancelOrderCommand;
        private readonly IConfirmOrderCommand _confirmOrderCommand;
        private readonly IChangePaymentMethodOrderCommand _changePaymentMethodOrderCommand;
        private readonly IDeleteDiscountOrderCommand _deleteDiscountOrderCommand;
        private readonly IRemovePaymentMethodOrderCommand _removePaymentMethodOrderCommand;
        private readonly IGetDiscountsOrderQuery _getDiscountsOrderQuery;
        private readonly IGetPaymentMethodOrderQuery _getPaymentMethodOrderQuery;

        public OrderController(
            ICreateOrderCommand createOrderCommand,
            IDeleteOrderCommand deleteOrderCommand,
            IUpdateOrderCommand updateOrderCommand,
            IGetOrderQuery getOrderQuery,
            IListOrderQuery listOrderQuery,
            IAddDiscountOrderCommand addDiscountOrderCommand,
            ICancelOrderCommand cancelOrderCommand,
            IConfirmOrderCommand confirmOrderCommand,
            IChangePaymentMethodOrderCommand changePaymentMethodOrderCommand,
            IDeleteDiscountOrderCommand deleteDiscountOrderCommand,
            IRemovePaymentMethodOrderCommand removePaymentMethodOrderCommand,
            IGetDiscountsOrderQuery getDiscountsOrderQuery,
            IGetPaymentMethodOrderQuery getPaymentMethodOrderQuery
        )
        {
            _createOrderCommand = createOrderCommand;
            _deleteOrderCommand = deleteOrderCommand;
            _updateOrderCommand = updateOrderCommand;
            _getOrderQuery = getOrderQuery;
            _listOrderQuery = listOrderQuery;
            _addDiscountOrderCommand = addDiscountOrderCommand;
            _cancelOrderCommand = cancelOrderCommand;
            _confirmOrderCommand = confirmOrderCommand;
            _changePaymentMethodOrderCommand = changePaymentMethodOrderCommand;
            _deleteDiscountOrderCommand = deleteDiscountOrderCommand;
            _removePaymentMethodOrderCommand = removePaymentMethodOrderCommand;
            _getDiscountsOrderQuery = getDiscountsOrderQuery;
            _getPaymentMethodOrderQuery = getPaymentMethodOrderQuery;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderResponse>> Post([FromBody] OrderRequest request)
        {
            var result = await _createOrderCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Discount")]
        [Authorize]
        public async Task<ActionResult<OrderDiscountResponse>> AddDiscount([FromBody] OrderDiscountRequest request)
        {
            var result = await _addDiscountOrderCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _deleteOrderCommand.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("Discount/{orderDiscountId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveDiscount(int orderDiscountId)
        {
            var result = await _deleteDiscountOrderCommand.Execute(orderDiscountId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<OrderResponse>> Put([FromBody] OrderRequest request)
        {
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

        [HttpPut("Confim/{orderId}")]
        [Authorize]
        public async Task<ActionResult<bool>> Confirm(int orderId)
        {
            var result = await _confirmOrderCommand.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("PaymentMethod/Change")]
        [Authorize]
        public async Task<ActionResult<OrderPaymentMethodResponse>> ChangePaymentMethod([FromBody] OrderPaymentMethodRequest request)
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

        [HttpGet("Discount/{orderId}")]
        [Authorize]
        public async Task<ActionResult<List<OrderDiscountDTO>>> GetDiscounts(int orderId)
        {
            var result = await _getDiscountsOrderQuery.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("PaymentMethod/{orderId}")]
        [Authorize]
        public async Task<ActionResult<OrderPaymentMethodResponse>> GetPaymentMethod(int orderId)
        {
            var result = await _getPaymentMethodOrderQuery.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }
    }
}
