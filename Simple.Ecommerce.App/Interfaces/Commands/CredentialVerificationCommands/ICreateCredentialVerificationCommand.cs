﻿using Simple.Ecommerce.Contracts.CredentialVerificationContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.CredentialVerificationCommands
{
    public interface ICreateCredentialVerificationCommand
    {
        Task<Result<CredentialVerificationResponse>> Execute(int loginId);
    }
}
