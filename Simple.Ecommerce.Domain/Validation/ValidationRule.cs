namespace Simple.Ecommerce.Domain.Validation
{
    public class ValidationRule
    {
        public Func<object, bool> Rule { get; }
        public string ErrorType { get; }
        public string ErrorMessage { get; }

        public ValidationRule(Func<object, bool> rule, string errorType, string errorMessage)
        {
            Rule = rule;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }
    }
}
