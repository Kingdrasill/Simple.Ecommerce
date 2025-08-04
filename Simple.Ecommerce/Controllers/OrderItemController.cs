using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Queries.OrderItemQueries;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Contracts.OrderItemContracts.Discounts;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IAddItemOrderItemCommand _addItemOrderItemCommand;
        private readonly IAddItemsOrderItemCommand _addItemsOrderItemCommand;
        private readonly IChangeDiscountOrderItemCommand _changeDiscountOrderItemCommand;
        private readonly IRemoveAllItemsOrderItemCommand _removeAllItemsOrderItemCommand;
        private readonly IRemoveItemOrderItemCommand _removeItemOrderItemCommand;
        private readonly IGetOrderItemQuery _getOrderItemQuery;
        private readonly IListOrderItemQuery _listOrderItemQuery;

        public OrderItemController(
            IAddItemOrderItemCommand addItemOrderItemCommand,
            IAddItemsOrderItemCommand addItemsOrderItemCommand,
            IChangeDiscountOrderItemCommand changeDiscountOrderItemCommand,
            IRemoveAllItemsOrderItemCommand removeAllItemsOrderItemCommand,
            IRemoveItemOrderItemCommand removeItemOrderItemCommand,
            IGetOrderItemQuery getOrderItemQuery, 
            IListOrderItemQuery listOrderItemQuery
        )
        {
            _addItemOrderItemCommand = addItemOrderItemCommand;
            _addItemsOrderItemCommand = addItemsOrderItemCommand;
            _changeDiscountOrderItemCommand = changeDiscountOrderItemCommand;
            _removeAllItemsOrderItemCommand = removeAllItemsOrderItemCommand;
            _removeItemOrderItemCommand = removeItemOrderItemCommand;
            _getOrderItemQuery = getOrderItemQuery;
            _listOrderItemQuery = listOrderItemQuery;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderItemsResponse>> Post([FromBody] OrderItemsRequest request)
        {
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

            var result = await _addItemsOrderItemCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("RemoveAll/{orderId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveAll(int orderId)
        {
            var result = await _removeAllItemsOrderItemCommand.Execute(orderId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Add")]
        [Authorize]
        public async Task<ActionResult<OrderItemResponse>> AddItem([FromBody] OrderItemRequest request)
        {
            if (request.CouponCode is not null && request.ProductDiscountId is not null)
            {
                return BadRequest($"OrderItemController.Conflict.Discount: Não se pode passar um código para coupon ({request.CouponCode}) e um desconto para produto ({request.ProductDiscountId}) ao mesmo tempo para um item de pedido!\n");
            }

            var result = await _addItemOrderItemCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Discount/Change")]
        [Authorize]
        public async Task<ActionResult<bool>> ChangeDiscount([FromBody] OrderItemDiscountRequest request)
        {
            if (request.CouponCode is not null && request.ProductDiscountId is not null)
            {
                return BadRequest($"OrderItemController.Conflict.Discount: Não se pode passar um código para coupon ({request.CouponCode}) e um desconto para produto ({request.ProductDiscountId}) ao mesmo tempo para um item de pedido!\n");
            }

            var result = await _changeDiscountOrderItemCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Remove")]
        [Authorize]
        public async Task<ActionResult<OrderItemResponse>> RemoveItem([FromBody] OrderItemRequest request)
        {
            if (request.CouponCode is not null && request.ProductDiscountId is not null)
            {
                return BadRequest($"OrderItemController.Conflict.Discount: Não se pode passar um código para coupon ({request.CouponCode}) e um desconto para produto ({request.ProductDiscountId}) ao mesmo tempo para um item de pedido!\n");
            }

            var result = await _removeItemOrderItemCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderItemResponse>> Get(int id)
        {
            var result = await _getOrderItemQuery.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<OrderItemResponse>>> List()
        {
            var result = await _listOrderItemQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }
    }
}
