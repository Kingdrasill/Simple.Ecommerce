﻿using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IGetDiscountDTOQuery
    {
        Task<Result<DiscountDTO>> Execute(int id);
    }
}
