﻿using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands
{
    public interface IChangeDiscountOrderItemCommand
    {
        Task<Result<bool>> Execute(OrderItemDiscountRequest request);
    }
}
