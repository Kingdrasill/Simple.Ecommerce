using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.BaseObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Microsoft.EntityFrameworkCore;

namespace Simple.Ecommerce.Domain.ValueObjects.PhotoObject
{
    [Owned]
    public class Photo : ValueObject
    {
        public Photo() { }

        private Photo(string fileName)
        {
            FileName = fileName;
        }

        public Result<Photo> Create(string fileName)
        {
            return new PhotoValidator().Validate(new Photo(fileName));
        }

        public string FileName { get; }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return FileName;
        }
    }
}
