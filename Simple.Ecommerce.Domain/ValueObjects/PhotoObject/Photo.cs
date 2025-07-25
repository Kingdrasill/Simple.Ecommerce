using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain.ValueObjects.BaseObject;

namespace Simple.Ecommerce.Domain.ValueObjects.PhotoObject
{
    [Owned]
    public class Photo : ValueObject
    {
        public Photo() { }

        public Photo(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return FileName;
        }
    }
}
