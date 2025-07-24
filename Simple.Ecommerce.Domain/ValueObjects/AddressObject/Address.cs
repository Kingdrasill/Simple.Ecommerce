using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.BaseObject;

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

        public int Number { get; set; }
        public string Street { get; set; }
        public string Neighbourhood { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string? Complement { get; set; }
        public string CEP { get; set; }

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
