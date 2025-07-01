using Simple.Ecommerce.Domain.Errors.BaseError;

namespace Simple.Ecommerce.Domain.Exceptions.ResultException
{
    public class ResultException : Exception
    {
        public List<Error> Errors { get; }

        public ResultException(List<Error> errors)
        {
            Errors = errors;
        }

        public ResultException(Error error)
        {
            Errors = new List<Error> { error };
        }
    }
}
