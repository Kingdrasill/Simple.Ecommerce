using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Queries.OrderItemQueries;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IAddItemsOrderItemCommand _addItemsOrderItemCommand;
        private readonly IGetOrderItemQuery _getOrderItemQuery;
        private readonly IListOrderItemQuery _listOrderItemQuery;

        public OrderItemController(
            IAddItemsOrderItemCommand addItemsOrderItemCommand,
            IGetOrderItemQuery getOrderItemQuery, 
            IListOrderItemQuery listOrderItemQuery
        )
        {
            _addItemsOrderItemCommand = addItemsOrderItemCommand;
            _getOrderItemQuery = getOrderItemQuery;
            _listOrderItemQuery = listOrderItemQuery;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderItemsResponse>> Post([FromBody] OrderItemsRequest request)
        {
            var result = await _addItemsOrderItemCommand.Execute(request);

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
