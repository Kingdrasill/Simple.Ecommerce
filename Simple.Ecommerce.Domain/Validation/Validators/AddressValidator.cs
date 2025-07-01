using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class AddressValidator : IBaseValidator<Address>
    {
        private readonly ValidationBuilder _builder;
    
        public AddressValidator()
        {
            _builder = new ValidationBuilder()
                .AddNegativeValueInt(nameof(Address.Number), typeof(Address).Name)
                .AddEmptyValue(nameof(Address.Street), typeof(Address).Name)
                .AddMaxLength(nameof(Address.Street), typeof(Address).Name, 30)
                .AddEmptyValue(nameof(Address.Neighbourhood), typeof(Address).Name)
                .AddMaxLength(nameof(Address.Neighbourhood), typeof(Address).Name, 30)
                .AddEmptyValue(nameof(Address.City), typeof(Address).Name)
                .AddMaxLength(nameof(Address.City), typeof(Address).Name, 30)
                .AddEmptyValue(nameof(Address.Country), typeof(Address).Name)
                .AddMaxLength(nameof(Address.Country), typeof(Address).Name, 30)
                .AddRule(nameof(Address.Complement), s => (string)s is { Length: > 30 }, $"{typeof(Address).Name}.MaxLength", "Não pode ter mais de 30 caracteres")
                .AddEmptyValue(nameof(Address.CEP), typeof(Address).Name)
                .AddRule(nameof(Address.CEP), s => ((string)s).Length != 8, $"{typeof(Address).Name}.WrongSize", "Precisa ser 8 caracteres!");
        }

        public Result<Address> Validate(Address entity)
        {
            var errors = _builder.Validate(entity);

            if (errors.Count != 0)
            {
                return Result<Address>.Failure(errors);
            }

            return Result<Address>.Success(entity);
        }
    }
}
