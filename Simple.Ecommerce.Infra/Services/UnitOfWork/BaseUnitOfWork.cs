using Microsoft.EntityFrameworkCore.Storage;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class BaseUnitOfWork : IBaseUnitOfWork
    {
        protected readonly TesteDbContext _context;
        private IDbContextTransaction? _transaction;

        protected BaseUnitOfWork(
            TesteDbContext context
        )
        {
            _context = context;
        }

        public virtual async Task BeginTransaction()
        {
            if (_transaction == null)
                _transaction = await _context.Database.BeginTransactionAsync();
        }

        public virtual async Task Commit()
        {
            if (_transaction != null)
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
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
