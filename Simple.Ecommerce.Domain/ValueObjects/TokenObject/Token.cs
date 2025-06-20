using Simple.Ecommerce.Domain.ValueObjects.BaseObject;

namespace Simple.Ecommerce.Domain.ValueObjects.TokenObject
{
    public class Token : ValueObject
    {
        public Token(string value, DateTime expiration)
        {
            Value = value;
            Expiration = expiration;
        }

        public string Value { get; }
        public DateTime Expiration { get; }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
            yield return Expiration;
        }
    }
}
