﻿using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class OrderValidator : IBaseValidator<Order>
    {
        private readonly ValidationBuilder _builder;

        public OrderValidator()
        {
            _builder = new ValidationBuilder();
        }

        public Result<Order> Validate(Order entity)
        {
            var errors = _builder.Validate(entity);

            if (errors.Count != 0)
            {
                return Result<Order>.Failure(errors);
            }

            return Result<Order>.Success(entity);
        }
    }
}
