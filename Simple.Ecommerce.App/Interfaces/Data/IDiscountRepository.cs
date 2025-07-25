﻿using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IDiscountRepository : 
        IBaseCreateRepository<Discount>,
        IBaseDeleteRepository<Discount>,
        IBaseGetRepository<Discount>,
        IBaseListRepository<Discount>,
        IBaseUpdateRepository<Discount>
    {
        Task<Result<List<Discount>>> GetByDiscountIds(List<int> ids);
    }
}
