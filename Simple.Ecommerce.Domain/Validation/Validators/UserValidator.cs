﻿using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Interfaces.BaseValidator;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class UserValidator : IBaseValidator<User>
    {
        private readonly ValidationBuilder _builder;

        public UserValidator()
        {
            _builder = new ValidationBuilder()
                .AddEmptyValue(nameof(User.Name), typeof(User).Name)
                .AddMaxLength(nameof(User.Name), typeof(User).Name, 50)
                .AddEmptyValue(nameof(User.Email), typeof(User).Name)
                .AddMaxLength(nameof(User.Email), typeof(User).Name, 50)
                .AddEmptyValue(nameof(User.PhoneNumber), typeof(User).Name)
                .AddMinLength(nameof(User.PhoneNumber), typeof(User).Name, 13)
                .AddMaxLength(nameof(User.PhoneNumber), typeof(User).Name, 14)
                .AddEmptyValue(nameof(User.Password), typeof(User).Name);
        }
            
        public Result<User> Validate(User entity)
        {
            var errors = _builder.Validate(entity);

            if (entity.Photo is not null)
            {
                var photoValidator = new PhotoValidator();
                var photoResult = photoValidator.Validate(entity.Photo);

                if (photoResult.IsFailure)
                    errors.AddRange(photoResult.Errors!);
            }

            return errors.Count != 0
                ? Result<User>.Failure(errors)
                : Result<User>.Success(entity);
        }
    }
}
