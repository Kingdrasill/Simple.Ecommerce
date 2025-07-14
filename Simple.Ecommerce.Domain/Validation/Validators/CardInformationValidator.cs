using Simple.Ecommerce.Domain.Interfaces.BaseValidator;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.ValueObjects.CardInformationObject;

namespace Simple.Ecommerce.Domain.Validation.Validators
{
    public class CardInformationValidator : IBaseValidator<CardInformation>
    {
        private readonly ValidationBuilder _builder;

        public CardInformationValidator()
        {
            _builder = new ValidationBuilder()
                .AddEmptyValue(nameof(CardInformation.CardHolderName), typeof(CardInformation).Name)
                .AddMaxLength(nameof(CardInformation.CardHolderName), typeof(CardInformation).Name, 50)
                .AddEmptyValue(nameof(CardInformation.CardNumber), typeof(CardInformation).Name)
                .AddEmptyValue(nameof(CardInformation.ExpirationMonth), typeof(CardInformation).Name)
                .AddMaxLength(nameof(CardInformation.ExpirationMonth), typeof(CardInformation).Name, 2)
                .AddEmptyValue(nameof(CardInformation.ExpirationYear), typeof(CardInformation).Name)
                .AddMaxLength(nameof(CardInformation.ExpirationYear), typeof(CardInformation).Name, 5);
        }

        public Result<CardInformation> Validate(CardInformation entity)
        {
            var erros = _builder.Validate(entity);

            if (erros.Count != 0)
            {
                return Result<CardInformation>.Failure(erros);
            }

            return Result<CardInformation>.Success(entity);
        }
    }
}
