using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.BaseObject;
using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.Domain.ValueObjects.AddressObject
{
    [Owned]
    public class Address : ValueObject
    {
        public Address() { }

        private Address(int number, string street, string neighbourhood, string city, string country, string? complement, string cep)
        {
            Number = number;
            Street = street;
            Neighbourhood = neighbourhood;
            City = city;
            Country = country;
            Complement = complement;
            CEP = cep;
        }

        public Result<Address> Create(int number, string street, string neighbourhood, string city, string country, string? complement, string cep)
        {
            return new AddressValidator().Validate(new Address(number, street, neighbourhood, city, country, complement, cep));
        }

        public int Number { get; }
        public string Street { get; }
        public string Neighbourhood { get; }
        public string City { get; }
        public string Country { get; }
        public string? Complement { get; }
        public string CEP { get; }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Number;
            yield return Street;
            yield return Neighbourhood;
            yield return City;
            yield return Country;
            yield return Complement;
            yield return CEP;
        }
    }
}
