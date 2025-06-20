using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.ReviewCommands;
using Simple.Ecommerce.App.Interfaces.Queries.ReviewQueries;
using Simple.Ecommerce.Contracts.ReviewContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ICreateReviewCommand _createReviewCommand;
        private readonly IDeleteReviewCommand _deleteReviewCommand;
        private readonly IUpdateReviewCommand _updateReviewCommand;
        private readonly IGetReviewQuery _getReviewQuery;
        private readonly IListReviewQuery _listReviewQuery;

        public ReviewController(
            ICreateReviewCommand createReviewCommand, 
            IDeleteReviewCommand deleteReviewCommand, 
            IUpdateReviewCommand updateReviewCommand, 
            IGetReviewQuery getReviewQuery, 
            IListReviewQuery listReviewQuery
        )
        {
            _createReviewCommand = createReviewCommand;
            _deleteReviewCommand = deleteReviewCommand;
            _updateReviewCommand = updateReviewCommand;
            _getReviewQuery = getReviewQuery;
            _listReviewQuery = listReviewQuery;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewResponse>> Post([FromBody] ReviewRequest request)
        {
            var result = await _createReviewCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _deleteReviewCommand.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ReviewResponse>> Put([FromBody] ReviewRequest request)
        {
            var result = await _updateReviewCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ReviewResponse>> Get(int id)
        {
            var result = await _getReviewQuery.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ReviewResponse>>> List()
        {
            var result = await _listReviewQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }
    }
}
