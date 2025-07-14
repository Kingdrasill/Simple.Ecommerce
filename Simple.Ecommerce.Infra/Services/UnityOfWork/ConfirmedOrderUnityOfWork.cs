using Microsoft.EntityFrameworkCore.Storage;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;

namespace Simple.Ecommerce.Infra.Services.UnityOfWork
{
    public class ConfirmedOrderUnityOfWork : IConfirmedOrderUnityOfWork
    {
        private readonly TesteDbContext _context;
        private IDbContextTransaction? _transaction;

        public IUserRepository Users { get; }
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }

        public ConfirmedOrderUnityOfWork(
            TesteDbContext context, 
            IUserRepository users, 
            IOrderRepository orders, 
            IOrderItemRepository orderItems
        )
        {
            _context = context;
            Users = users;
            Orders = orders;
            OrderItems = orderItems;
        }

        public async Task BeginTransaction()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task Commit()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await Rollback();
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }


        public async Task Rollback()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
