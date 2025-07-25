﻿using Simple.Ecommerce.Contracts.ReviewContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.ReviewCommands
{
    public interface ICreateReviewCommand
    {
        Task<Result<ReviewResponse>> Execute(ReviewRequest request);
    }
}
