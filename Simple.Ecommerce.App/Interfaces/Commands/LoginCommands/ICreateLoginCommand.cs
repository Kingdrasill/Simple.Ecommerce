﻿using Simple.Ecommerce.Contracts.LoginContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.LoginCommands
{
    public interface ICreateLoginCommand
    {
        Task<Result<bool>> Execute(LoginRequest request);
    }
}
