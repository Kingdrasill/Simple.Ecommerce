namespace Simple.Ecommerce.Domain.ValueObjects.BaseObject
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        public abstract IEnumerable<object?> GetEqualityComponents();

        public bool Equals(ValueObject? other)
        {
            return other is not null && ValuesAreEqual(other);
        }

        public override bool Equals(object? obj)
        {
            return obj is ValueObject valueObject && ValuesAreEqual(valueObject);
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Aggregate(
                    default(int),
                    HashCode.Combine);
        }

        public bool ValuesAreEqual(ValueObject other)
        {
            return other.GetEqualityComponents().SequenceEqual(GetEqualityComponents());
        }
    }
}
