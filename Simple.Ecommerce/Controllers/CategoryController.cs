using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.CategoryCommands;
using Simple.Ecommerce.App.Interfaces.Queries.CategoryQueries;
using Simple.Ecommerce.Contracts.CategoryContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICreateCategoryCommand _createCategoryCommand;
        private readonly IDeleteCategoryCommand _deleteCategoryCommand;
        private readonly IUpdateCategoryCommand _updateCategoryCommand;
        private readonly IGetCategoryQuery _getCategoryQuery;
        private readonly IListCategoryQuery _listCategoryQuery;

        public CategoryController(
            ICreateCategoryCommand createCategoryCommand, 
            IDeleteCategoryCommand deleteCategoryCommand, 
            IUpdateCategoryCommand updateCategoryCommand, 
            IGetCategoryQuery getCategoryQuery, 
            IListCategoryQuery listCategoryQuery
        )
        {
            _createCategoryCommand = createCategoryCommand;
            _deleteCategoryCommand = deleteCategoryCommand;
            _updateCategoryCommand = updateCategoryCommand;
            _getCategoryQuery = getCategoryQuery;
            _listCategoryQuery = listCategoryQuery;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoryResponse>> Post([FromBody] CategoryRequest request)
        {
            var result = await _createCategoryCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _deleteCategoryCommand.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<CategoryResponse>> Put([FromBody] CategoryRequest request)
        {
            var result = await _updateCategoryCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CategoryResponse>> Get(int id)
        {
            var result = await _getCategoryQuery.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<CategoryResponse>>> List()
        {
            var result = await _listCategoryQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }
    }
}
