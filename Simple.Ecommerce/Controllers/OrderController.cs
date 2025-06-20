using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Queries.OrderQueries;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Contracts.OrderDiscountContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IAddDiscountOrderCommand _addDiscountOrderCommand;
        private readonly IDeleteDiscountOrderCommand _deleteDiscountOrderCommand;
        private readonly IGetDiscountsOrderQuery _getDiscountsOrderQuery;

        public OrderController(
            ICreateOrderCommand createOrderCommand, 
            IDeleteOrderCommand deleteOrderCommand, 
            IUpdateOrderCommand updateOrderCommand, 
            IGetOrderQuery getOrderQuery, 
            IListOrderQuery listOrderQuery,
            ICancelOrderCommand cancelOrderCommand,
            IConfirmOrderCommand confirmOrderCommand,
            IAddDiscountOrderCommand addDiscountOrderCommand,
            IDeleteDiscountOrderCommand deleteDiscountOrderCommand,
            IGetDiscountsOrderQuery getDiscountsOrderQuery
        )
        {
            _createOrderCommand = createOrderCommand;
            _deleteOrderCommand = deleteOrderCommand;
            _updateOrderCommand = updateOrderCommand;
            _getOrderQuery = getOrderQuery;
            _listOrderQuery = listOrderQuery;
            _cancelOrderCommand = cancelOrderCommand;
            _confirmOrderCommand = confirmOrderCommand;
            _addDiscountOrderCommand = addDiscountOrderCommand;
            _deleteDiscountOrderCommand = deleteDiscountOrderCommand;
            _getDiscountsOrderQuery = getDiscountsOrderQuery;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderResponse>> Post([FromBody] OrderRequest request)
        {
            var result = await _createOrderCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Add-Discount")]
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

        [HttpDelete("Remove-Discount/{orderDiscountId}")]
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

        [HttpGet("Get-Discounts/{orderId}")]
        [Authorize]
        public async Task<ActionResult<List<OrderDiscountDTO>>> GetDiscounts(int orderId)
        {
            var result = await _getDiscountsOrderQuery.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }
    }
}
