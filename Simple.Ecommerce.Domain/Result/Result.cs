using Simple.Ecommerce.Domain.Errors.BaseError;

namespace Simple.Ecommerce.Domain
{
    public class Result<T>
    {
        private readonly T? _value;
        public List<Error>? Errors { get; private set; }
        public bool IsSuccess { get; private set; }
        public bool IsFailure { get; private set; }

        public Result(T? value, List<Error>? errors, bool isSuccess, bool isFailure)
        {
            _value = value;
            Errors = errors;
            IsSuccess = isSuccess;
            IsFailure = isFailure;
        }

        public T GetValue()
        {
            return _value!;
        }

        public static Result<T> Failure(List<Error> errors) =>
            new(default, errors, false, true);

        public static Result<T> Success(T value) => 
            new(value, null, true, false);
    }
}
