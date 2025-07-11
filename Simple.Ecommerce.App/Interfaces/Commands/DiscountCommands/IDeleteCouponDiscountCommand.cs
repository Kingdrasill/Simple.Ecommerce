﻿using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IDeleteCouponDiscountCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}
