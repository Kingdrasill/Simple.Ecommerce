using Microsoft.EntityFrameworkCore.Storage;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.Infra.UoW
{
    public class SaverTransectioner : ISaverTransectioner
    {
        private readonly TesteDbContext _context;
        private IDbContextTransaction? _transaction;

        public SaverTransectioner(TesteDbContext context)
        {
            _context = context;
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

        public async Task<Result<bool>> SaveChanges()
        {
            try
            {
                await _context.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(new List<Error> { new("UnityOfWork.SaveChanges", ex.Message) });
            }
        }
    }
}
