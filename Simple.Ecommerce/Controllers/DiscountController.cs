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
        private readonly IToggleActivationDiscountCommand _toggleActivationDiscountCommand;
        private readonly IGetDiscountDTOQuery _getDiscountDTOQuery;
        private readonly IListDiscountDTOQuery _listDiscountDTOQuery;
        private readonly ICreateDiscountCommand _createDiscountCommand;
        private readonly IDeleteDiscountCommand _deleteDiscountCommand;
        private readonly IUpdateDiscountCommand _updateDiscountCommand;
        private readonly IGetDiscountQuery _getDiscountQuery;
        private readonly IListDiscountQuery _listDiscountQuery;
        private readonly ICreateBatchCouponsDiscountCommand _createBatchCouponsDiscountCommand;
        private readonly IDeleteCouponDiscountCommand _deleteCouponDiscountCommand;
        private readonly IUpdateCouponDiscountCommand _updateCouponDiscountCommand;
        private readonly IGetCouponDiscountQuery _getCouponDiscountQuery;
        private readonly IListCouponDiscountQuery _listCouponDiscountQuery;
        private readonly ICreateBundleItemDiscountCommand _createDiscountBundleItemDiscountCommand;
        private readonly IDeleteBundleItemDiscountCommand _deleteDiscountBundleItemDiscountCommand;
        private readonly IUpdateBundleItemDiscountCommand _updateDiscountBundleItemDiscountCommand;
        private readonly IListBundleItemDiscountQuery _listDiscountBundleItemDiscountQuery;
        private readonly IGetBundleItemDiscountQuery _getDiscountBundleItemDiscountQuery;
        private readonly ICreateTierDiscountCommand _createDiscountTierDiscountCommand;
        private readonly IDeleteTierDiscountCommand _deleteDiscountTierDiscountCommand;
        private readonly IUpdateTierDiscountCommand _updateDicountTierDiscountCommand;
        private readonly IGetTierDiscountQuery _getDiscountTierDiscountQuery;
        private readonly IListTierDiscountQuery _listDiscountTierDiscountQuery;

        public DiscountController(
            IToggleActivationDiscountCommand toggleActivationDiscountCommand,
            IGetDiscountDTOQuery getDiscountDTOQuery,
            IListDiscountDTOQuery listDiscountDTOQuery,
            ICreateDiscountCommand createDiscountCommand,
            IDeleteDiscountCommand deleteDiscountCommand,
            IUpdateDiscountCommand updateDiscountCommand,
            IGetDiscountQuery getDiscountQuery,
            IListDiscountQuery listDiscountQuery,
            ICreateBatchCouponsDiscountCommand createBatchCouponsDiscountCommand,
            IDeleteCouponDiscountCommand deleteCouponDiscountCommand,
            IUpdateCouponDiscountCommand updateCouponDiscountCommand,
            IGetCouponDiscountQuery getCouponDiscountQuery,
            IListCouponDiscountQuery listCouponDiscountQuery,
            ICreateBundleItemDiscountCommand createDiscountBundleItemDiscountCommand,
            IDeleteBundleItemDiscountCommand deleteDiscountBundleItemDiscountCommand,
            IUpdateBundleItemDiscountCommand updateDiscountBundleItemDiscountCommand,
            IGetBundleItemDiscountQuery getDiscountBundleItemDiscountQuery,
            IListBundleItemDiscountQuery listDiscountBundleItemDiscountQuery,
            ICreateTierDiscountCommand createDiscountTierDiscountCommand,
            IDeleteTierDiscountCommand deleteDiscountTierDiscountCommand,
            IUpdateTierDiscountCommand updateDicountTierDiscountCommand,
            IGetTierDiscountQuery getDiscountTierDiscountQuery,
            IListTierDiscountQuery listDiscountTierDiscountQuery
        )
        {
            _toggleActivationDiscountCommand = toggleActivationDiscountCommand;
            _getDiscountDTOQuery = getDiscountDTOQuery;
            _listDiscountQuery = listDiscountQuery;
            _createDiscountCommand = createDiscountCommand;
            _deleteDiscountCommand = deleteDiscountCommand;
            _updateDiscountCommand = updateDiscountCommand;
            _getDiscountQuery = getDiscountQuery;
            _listDiscountQuery = listDiscountQuery;
            _createBatchCouponsDiscountCommand = createBatchCouponsDiscountCommand;
            _deleteCouponDiscountCommand = deleteCouponDiscountCommand;
            _updateCouponDiscountCommand = updateCouponDiscountCommand;
            _getCouponDiscountQuery = getCouponDiscountQuery;
            _listCouponDiscountQuery = listCouponDiscountQuery;
            _createDiscountBundleItemDiscountCommand = createDiscountBundleItemDiscountCommand;
            _deleteDiscountBundleItemDiscountCommand = deleteDiscountBundleItemDiscountCommand;
            _updateDiscountBundleItemDiscountCommand = updateDiscountBundleItemDiscountCommand;
            _getDiscountBundleItemDiscountQuery = getDiscountBundleItemDiscountQuery;
            _listDiscountBundleItemDiscountQuery = listDiscountBundleItemDiscountQuery;
            _createDiscountTierDiscountCommand = createDiscountTierDiscountCommand;
            _deleteDiscountTierDiscountCommand= deleteDiscountTierDiscountCommand;
            _updateDicountTierDiscountCommand = updateDicountTierDiscountCommand;
            _getDiscountTierDiscountQuery = getDiscountTierDiscountQuery;
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

        [HttpPut("ToggleActivation/{id}/{isActive}")]
        public async Task<ActionResult<bool>> ToggleActivation(int id, bool isActive)
        {
            var result = await _toggleActivationDiscountCommand.Execute(id, isActive);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut("Coupon")]
        [Authorize]
        public async Task<ActionResult<CouponResponse>> UpdateCoupon([FromBody] CouponRequest request)
        {
            var result = await _updateCouponDiscountCommand.Execute(request);

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
        public async Task<ActionResult<DiscountCompleteDTO>> GetDTO(int id)
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
        public async Task<ActionResult<List<DiscountCompleteDTO>>> ListDTO()
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
