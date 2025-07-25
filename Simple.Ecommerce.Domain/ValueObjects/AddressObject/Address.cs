using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain.ValueObjects.BaseObject;

namespace Simple.Ecommerce.Domain.ValueObjects.AddressObject
{
    [Owned]
    public class Address : ValueObject
    {
        public Address() { }

        public Address(int number, string street, string neighbourhood, string city, string country, string? complement, string cep)
        {
            Number = number;
            Street = street;
            Neighbourhood = neighbourhood;
            City = city;
            Country = country;
            Complement = complement;
            CEP = cep;
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
