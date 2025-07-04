﻿using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Data.BaseRepository
{
    public interface IBaseDeleteRepository<T> where T : class
    {
        Task<Result<bool>> Delete(int id);
    }
}
