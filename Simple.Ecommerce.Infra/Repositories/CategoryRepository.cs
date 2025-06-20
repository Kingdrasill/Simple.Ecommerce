using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;
using Microsoft.EntityFrameworkCore;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<Category> _createRepository;
        private readonly IGenericDeleteRepository<Category> _deleteRepository;
        private readonly IGenericGetRepository<Category> _getRepository;
        private readonly IGenericListRepository<Category> _listRepository;
        private readonly IGenericUpdateRepository<Category> _updateRepository;

        public CategoryRepository(
            TesteDbContext context, 
            IGenericCreateRepository<Category> createRepository, 
            IGenericDeleteRepository<Category> deleteRepository, 
            IGenericGetRepository<Category> getRepository, 
            IGenericListRepository<Category> listRepository, 
            IGenericUpdateRepository<Category> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<Category>> Create(Category entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<Category>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<Category>>> GetCategoriesByIds(List<int> ids)
        {
            var categories = await _context.Categories
                .Where(c => ids.Contains(c.Id) && !c.Deleted)
                .ToListAsync();

            if (categories.Count != ids.Count)
            {
                return Result<List<Category>>.Failure(new List<Error> { new Error("Category.NotFound", "Uma ou mais categorias não foram encontradas!") });
            }

            return Result<List<Category>>.Success(categories);
        }

        public async Task<Result<List<Category>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<Category>> Update(Category entity)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}
