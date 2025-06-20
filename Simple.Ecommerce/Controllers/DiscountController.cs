using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly ICreateDiscountCommand _createDiscountCommand;
        private readonly ICreateBatchCouponsDiscountCommand _createBatchCouponsDiscountCommand;
        private readonly ICreateDiscountBundleItemDiscountCommand _createDiscountBundleItemDiscountCommand;
        private readonly ICreateDiscountTierDiscountCommand _createDiscountTierDiscountCommand;
        private readonly IDeleteDiscountCommand _deleteDiscountCommand;
        private readonly IDeleteCouponDiscountCommand _deleteCouponDiscountCommand;
        private readonly IDeleteDiscountBundleItemDiscountCommand _deleteDiscountBundleItemDiscountCommand;
        private readonly IDeleteDiscountTierDiscountCommand _deleteDiscountTierDiscountCommand;
        private readonly IUpdateDiscountCommand _updateDiscountCommand;
        private readonly IUpdateCouponDiscountCommand _updateCouponDiscountCommand;
        private readonly IUpdateDiscountBundleItemDiscountCommand _updateDiscountBundleItemDiscountCommand;
        private readonly IUpdateDiscountTierDiscountCommand _updateDicountTierDiscountCommand;
        private readonly IUseCouponDiscountCommand _useCouponDiscountCommand;
        private readonly IGetDiscountQuery _getDiscountQuery;
        private readonly IGetDiscountDTOQuery _getDiscountDTOQuery;
        private readonly IGetCouponDiscountQuery _getCouponDiscountQuery;
        private readonly IGetDiscountBundleItemDiscountQuery _getDiscountBundleItemDiscountQuery;
        private readonly IGetDiscountTierDiscountQuery _getDiscountTierDiscountQuery;
        private readonly IListDiscountQuery _listDiscountQuery;
        private readonly IListDiscountDTOQuery _listDiscountDTOQuery;
        private readonly IListCouponDiscountQuery _listCouponDiscountQuery;
        private readonly IListDiscountBundleItemDiscountQuery _listDiscountBundleItemDiscountQuery;
        private readonly IListDiscountTierDiscountQuery _listDiscountTierDiscountQuery;

        public DiscountController(
            ICreateDiscountCommand createDiscountCommand,
            ICreateBatchCouponsDiscountCommand createBatchCouponsDiscountCommand,
            ICreateDiscountBundleItemDiscountCommand createDiscountBundleItemDiscountCommand, 
            ICreateDiscountTierDiscountCommand createDiscountTierDiscountCommand,
            IDeleteDiscountCommand deleteDiscountCommand,
            IDeleteCouponDiscountCommand deleteCouponDiscountCommand,
            IDeleteDiscountBundleItemDiscountCommand deleteDiscountBundleItemDiscountCommand, 
            IDeleteDiscountTierDiscountCommand deleteDiscountTierDiscountCommand,
            IUpdateDiscountCommand updateDiscountCommand,
            IUpdateCouponDiscountCommand updateCouponDiscountCommand,
            IUpdateDiscountBundleItemDiscountCommand updateDiscountBundleItemDiscountCommand, 
            IUpdateDiscountTierDiscountCommand updateDicountTierDiscountCommand,
            IUseCouponDiscountCommand useCouponDiscountCommand,
            IGetDiscountQuery getDiscountQuery,
            IGetDiscountDTOQuery getDiscountDTOQuery,
            IGetCouponDiscountQuery getCouponDiscountQuery,
            IGetDiscountBundleItemDiscountQuery getDiscountBundleItemDiscountQuery, 
            IGetDiscountTierDiscountQuery getDiscountTierDiscountQuery,
            IListDiscountQuery listDiscountQuery,
            IListDiscountDTOQuery listDiscountDTOQuery,
            IListCouponDiscountQuery listCouponDiscountQuery,
            IListDiscountBundleItemDiscountQuery listDiscountBundleItemDiscountQuery, 
            IListDiscountTierDiscountQuery listDiscountTierDiscountQuery
        )
        {
            _createDiscountCommand = createDiscountCommand;
            _createBatchCouponsDiscountCommand = createBatchCouponsDiscountCommand;
            _createDiscountBundleItemDiscountCommand = createDiscountBundleItemDiscountCommand;
            _createDiscountTierDiscountCommand = createDiscountTierDiscountCommand;
            _deleteDiscountCommand = deleteDiscountCommand;
            _deleteCouponDiscountCommand = deleteCouponDiscountCommand;
            _deleteDiscountBundleItemDiscountCommand = deleteDiscountBundleItemDiscountCommand;
            _deleteDiscountTierDiscountCommand = deleteDiscountTierDiscountCommand;
            _updateDiscountCommand = updateDiscountCommand;
            _updateCouponDiscountCommand = updateCouponDiscountCommand;
            _updateDiscountBundleItemDiscountCommand = updateDiscountBundleItemDiscountCommand;
            _updateDicountTierDiscountCommand = updateDicountTierDiscountCommand;
            _useCouponDiscountCommand = useCouponDiscountCommand;
            _getDiscountQuery = getDiscountQuery;
            _getDiscountDTOQuery = getDiscountDTOQuery;
            _getCouponDiscountQuery = getCouponDiscountQuery;
            _getDiscountBundleItemDiscountQuery = getDiscountBundleItemDiscountQuery;
            _getDiscountTierDiscountQuery = getDiscountTierDiscountQuery;
            _listDiscountQuery = listDiscountQuery;
            _listDiscountDTOQuery = listDiscountDTOQuery;
            _listCouponDiscountQuery = listCouponDiscountQuery;
            _listDiscountBundleItemDiscountQuery = listDiscountBundleItemDiscountQuery;
            _listDiscountTierDiscountQuery = listDiscountTierDiscountQuery;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<DiscountResponse>> Create([FromBody] DiscountRequest request)
        {
            var result = await _createDiscountCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Coupon")]
        [Authorize]
        public async Task<ActionResult<List<CouponResponse>>> CreateBathCoupons([FromBody] CouponBatchRequest request)
        {
            var result = await _createBatchCouponsDiscountCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("DiscountBundleItem")]
        [Authorize]
        public async Task<ActionResult<DiscountBundleItemResponse>> CreateDiscountBundleItem([FromBody] DiscountBundleItemRequest request)
        {
            var result = await _createDiscountBundleItemDiscountCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("DiscountTier")]
        [Authorize]
        public async Task<ActionResult<DiscountTierResponse>> CreateDiscountTier([FromBody] DiscountTierRequest request)
        {
            var result = await _createDiscountTierDiscountCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _deleteDiscountCommand.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("Coupon/{couponId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteCoupon(int couponId)
        {
            var result = await _deleteCouponDiscountCommand.Execute(couponId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("DiscountBundleItem/{discountBundleItemId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteDiscountBundleItem(int discountBundleItemId)
        {
            var result = await _deleteDiscountBundleItemDiscountCommand.Execute(discountBundleItemId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("DiscountTier/{discountTierId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteDiscountTier(int discountTierId)
        {
            var result = await _deleteDiscountTierDiscountCommand.Execute(discountTierId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<DiscountResponse>> Update([FromBody] DiscountRequest request)
        {
            var result = await _updateDiscountCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Coupon")]
        [Authorize]
        public async Task<ActionResult<CouponResponse>> UpdateCoupon([FromBody] CouponRequest request)
        {
            var result = await _updateCouponDiscountCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Coupon/Use-Coupon/{token}")]
        [Authorize]
        public async Task<ActionResult<bool>> UseCoupon(string token)
        {
            var result = await _useCouponDiscountCommand.Execute(token);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("DiscountBundleItem")]
        [Authorize]
        public async Task<ActionResult<DiscountBundleItemResponse>> UpdateDiscountBundleItem([FromBody] DiscountBundleItemRequest request)
        {
            var result = await _updateDiscountBundleItemDiscountCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("DiscountTier")]
        [Authorize]
        public async Task<ActionResult<DiscountTierResponse>> UpdateDiscountTier([FromBody] DiscountTierRequest request)
        {
            var result = await _updateDicountTierDiscountCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<DiscountResponse>> Get(int id)
        {
            var result = await _getDiscountQuery.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("DiscountDTO/{id}")]
        [Authorize]
        public async Task<ActionResult<DiscountDTO>> GetDTO(int id)
        {
            var result = await _getDiscountDTOQuery.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("Coupon/{couponId}")]
        [Authorize]
        public async Task<ActionResult<CouponResponse>> GetCoupon(int couponId)
        {
            var result = await _getCouponDiscountQuery.Execute(couponId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("DiscountBundleItem/{discountBundleItemId}")]
        [Authorize]
        public async Task<ActionResult<DiscountBundleItemResponse>> GetDiscountBundleItem(int discountBundleItemId)
        {
            var result = await _getDiscountBundleItemDiscountQuery.Execute(discountBundleItemId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("DiscountTier/{discountTierId}")]
        [Authorize]
        public async Task<ActionResult<DiscountTierResponse>> GetDiscountTier(int discountTierId)
        {
            var result = await _getDiscountTierDiscountQuery.Execute(discountTierId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<DiscountResponse>>> List()
        {
            var result = await _listDiscountQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("DiscountDTO")]
        [Authorize]
        public async Task<ActionResult<List<DiscountDTO>>> ListDTO()
        {
            var result = await _listDiscountDTOQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("Coupon")]
        [Authorize]
        public async Task<ActionResult<List<CouponResponse>>> ListCoupon()
        {
            var result = await _listCouponDiscountQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("DiscountBundleItem")]
        [Authorize]
        public async Task<ActionResult<List<DiscountBundleItemResponse>>> ListDiscountBundleItem()
        {
            var result = await _listDiscountBundleItemDiscountQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("DiscountTier")]
        [Authorize]
        public async Task<ActionResult<List<DiscountTierResponse>>> ListDiscountTier()
        {
            var result = await _listDiscountTierDiscountQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }
    }
}
