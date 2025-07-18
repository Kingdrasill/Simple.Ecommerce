﻿using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Data.BaseRepository
{
    public interface IBaseCreateRepository<T> where T : class
    {
        Task<Result<T>> Create(T entity, bool skipSave = false);
    }
}
