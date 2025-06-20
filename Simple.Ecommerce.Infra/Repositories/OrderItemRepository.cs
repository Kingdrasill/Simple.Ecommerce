using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<OrderItem> _createRepository;
        private readonly IGenericDeleteRepository<OrderItem> _deleteRepository;
        private readonly IGenericGetRepository<OrderItem> _getRepository;
        private readonly IGenericListRepository<OrderItem> _listRepository;

        public OrderItemRepository(
            TesteDbContext context, 
            IGenericCreateRepository<OrderItem> createRepository,
            IGenericDeleteRepository<OrderItem> deleteRepository, 
            IGenericGetRepository<OrderItem> getRepository, 
            IGenericListRepository<OrderItem> listRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
        }

        public async Task<Result<OrderItem>> Create(OrderItem entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<OrderItem>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<OrderItem>>> List()
        {
            return await _listRepository.List(_context);
        }
    }
}
