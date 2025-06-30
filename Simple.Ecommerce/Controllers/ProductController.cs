using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Queries.ProductQueries;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.ProductCategoryContracts;
using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Contracts.ProductDiscountContracts;
using Simple.Ecommerce.Contracts.ProductPhotoContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ICreateProductCommand _createProductCommand;
        private readonly IDeleteProductCommand _deleteProductCommand;
        private readonly IUpdateProductCommand _updateProductCommand;
        private readonly IGetProductQuery _getProductQuery;
        private readonly IListProductQuery _listProductQuery;
        private readonly IAddCategoryProductCommand _addCategoryProductCommand;
        private readonly IAddDiscountProductCommand _addDiscountProductCommand;
        private readonly IAddPhotoProductCommand _addProductPhotoCommand;
        private readonly IRemoveCategoryProductCommand _removeCategoryProductCommand;
        private readonly IRemoveDiscountProductCommand _removeDiscountProductCommand;
        private readonly IRemovePhotoProductCommand _removeProductPhotoCommand;
        private readonly IGetCategoriesProductQuery _getCategoriesProductQuery;
        private readonly IGetDiscountsProductQuery _getDiscountsProductQuery;
        private readonly IGetPhotosProductQuery _getPhotosProductQuery;

        public ProductController(
            ICreateProductCommand createProductCommand, 
            IDeleteProductCommand deleteProductCommand, 
            IUpdateProductCommand updateProductCommand, 
            IGetProductQuery getProductQuery, 
            IListProductQuery listProductQuery,
            IAddCategoryProductCommand addCategoryProductCommand,
            IAddDiscountProductCommand addDiscountProductCommand,
            IAddPhotoProductCommand addProductPhotoCommand,
            IRemoveCategoryProductCommand removeCategoryProductCommand,
            IRemoveDiscountProductCommand removeDiscountProductCommand,
            IRemovePhotoProductCommand removeProductPhotoCommand,
            IGetCategoriesProductQuery getCategoriesProductQuery,
            IGetDiscountsProductQuery getDiscountsProductQuery,
            IGetPhotosProductQuery getPhotosProductQuery
        )
        {
            _createProductCommand = createProductCommand;
            _deleteProductCommand = deleteProductCommand;
            _updateProductCommand = updateProductCommand;
            _getProductQuery = getProductQuery;
            _listProductQuery = listProductQuery;
            _addCategoryProductCommand = addCategoryProductCommand;
            _addDiscountProductCommand = addDiscountProductCommand;
            _addProductPhotoCommand = addProductPhotoCommand;
            _removeCategoryProductCommand = removeCategoryProductCommand;
            _removeDiscountProductCommand = removeDiscountProductCommand;
            _removeProductPhotoCommand = removeProductPhotoCommand;
            _getCategoriesProductQuery = getCategoriesProductQuery;
            _getDiscountsProductQuery = getDiscountsProductQuery;
            _getPhotosProductQuery = getPhotosProductQuery;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProductResponse>> Post([FromBody] ProductRequest request)
        {
            var result = await _createProductCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Category")]
        [Authorize]
        public async Task<ActionResult<ProductCategoryDTO>> AddCategory([FromBody] ProductCategoryRequest request)
        {
            var result = await _addCategoryProductCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Discount")]
        [Authorize]
        public async Task<ActionResult<ProductDiscountResponse>> AddDiscount([FromBody] ProductDiscountRequest request)
        {
            var result = await _addDiscountProductCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Photo")]
        [Authorize]
        public async Task<ActionResult<ProductPhotoResponse>> AddPhoto([FromForm] ProductPhotoUploadRequest request)
        {
            if (request.File is null || request.File.Length == 0)
                return Problem("Nenhum arquivo foi enviado.");

            var allowdExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(request.File.FileName).ToLower();
            if (!allowdExtensions.Contains(extension))
                return Problem("O formato da imagem não é suportado!");

            using var stream = request.File.OpenReadStream();
            var ppRequest = new ProductPhotoRequest(request.ProductId, request.Compress, request.Deletable);

            var result = await _addProductPhotoCommand.Execute(ppRequest, stream, extension);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _deleteProductCommand.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("Category/{productCategoryId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveCategory(int productCategoryId)
        {
            var result = await _removeCategoryProductCommand.Execute(productCategoryId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("Discount/{productDiscountId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveDiscount(int productDiscountId)
        {
            var result = await _removeDiscountProductCommand.Execute(productDiscountId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("Photo/{productPhotoId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemovePhoto(int productPhotoId)
        {
            var result = await _removeProductPhotoCommand.Execute(productPhotoId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ProductResponse>> Put([FromBody] ProductRequest request)
        {
            var result = await _updateProductCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductResponse>> Get(int id)
        {
            var result = await _getProductQuery.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ProductResponse>>> List()
        {
            var result = await _listProductQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("Category/{productId}")]
        [Authorize]
        public async Task<ActionResult<ProductCategoriesDTO>> GetCategories(int productId)
        {
            var result = await _getCategoriesProductQuery.Execute(productId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("Discount/{productId}")]
        [Authorize]
        public async Task<ActionResult<List<DiscountDTO>>> GetDiscounts(int productId)
        {
            var result = await _getDiscountsProductQuery.Execute(productId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("Photo/{productId}")]
        [Authorize]
        public async Task<ActionResult<ProductPhotosResponse>> GetPhotos(int productId)
        {
            var result = await _getPhotosProductQuery.Execute(productId);

            return ResultHandler.HandleResult(this, result);
        }
    }
}
