﻿using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Data.BaseRepository
{
    public interface IBaseDeleteRepository<T> where T : class
    {
        Task<Result<bool>> Delete(int id, bool skipSave = false);
    }
}
