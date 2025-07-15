using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<Coupon> _createRepository;
        private readonly IGenericDeleteRepository<Coupon> _deleteRepository;
        private readonly IGenericGetRepository<Coupon> _getRepository;
        private readonly IGenericListRepository<Coupon> _listRepository;
        private readonly IGenericUpdateRepository<Coupon> _updateRepository;

        public CouponRepository(
            TesteDbContext context,
            IGenericCreateRepository<Coupon> createRepository,
            IGenericDeleteRepository<Coupon> deleteRepository,
            IGenericGetRepository<Coupon> getRepository,
            IGenericListRepository<Coupon> listRepository,
            IGenericUpdateRepository<Coupon> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<Coupon>> Create(Coupon entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity, skipSave);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<Coupon>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<Coupon>> GetByCode(string code)
        {
            IQueryable<Coupon> query = _context.Coupons.AsNoTracking();
            var coupon = await query.FirstOrDefaultAsync(c => c.Code == code && !c.Deleted);

            if (coupon is null)
                return Result<Coupon>.Failure(new List<Error> { new Error("Coupon.NotFound", "O cupom não foi encontrado!") });
            return Result<Coupon>.Success(coupon);
        }

        public async Task<Result<List<Coupon>>> GetByDiscountId(int discountId)
        {
            var coupons = await _context.Coupons
                                            .Where(c => c.DiscountId == discountId && !c.Deleted)
                                            .ToListAsync();

            return Result<List<Coupon>>.Success(coupons);
        }

        public async Task<Result<List<Coupon>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<Coupon>> Update(Coupon entity, bool skipSave = false)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}
