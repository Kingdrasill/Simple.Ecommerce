using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<Order> _createRepository;
        private readonly IGenericDeleteRepository<Order> _deleteRepository;
        private readonly IGenericGetRepository<Order> _getRepository;
        private readonly IGenericListRepository<Order> _listRepository;
        private readonly IGenericUpdateRepository<Order> _updateRepository;

        public OrderRepository(
            TesteDbContext context, 
            IGenericCreateRepository<Order> createRepository, 
            IGenericDeleteRepository<Order> deleteRepository, 
            IGenericGetRepository<Order> getRepository, 
            IGenericListRepository<Order> listRepository, 
            IGenericUpdateRepository<Order> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<Order>> Create(Order entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<Order>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<Order>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<Order>> Update(Order entity)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}
