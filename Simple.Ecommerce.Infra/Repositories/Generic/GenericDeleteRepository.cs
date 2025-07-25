﻿using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Interfaces.BaseEntity;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories.Generic
{
    public class GenericDeleteRepository<T> : IGenericDeleteRepository<T> where T : class, IBaseEntity
    {
        public virtual async Task<Result<bool>> Delete(TesteDbContext context, int id, bool skipSave = false)
        {
            var entity = await context.Set<T>().FindAsync(id);

            if (entity is null)
                return Result<bool>.Failure(new List<Error> { new Error("NotFound", "Entidade não foi encontrada!") });

            entity.MarkAsDeleted();

            context.Set<T>().Update(entity);
            if (!skipSave)
                await context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
