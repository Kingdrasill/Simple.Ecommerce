using Simple.Ecommerce.Domain.Errors.BaseError;

namespace Simple.Ecommerce.Domain.Validation
{
    public class ValidationBuilder
    {
        private readonly Dictionary<string, List<ValidationRule>> _rules = new();

        public ValidationBuilder AddRule(string propertyName, Func<object, bool> rule, string errorType, string errorMessage) 
        {
            if (!_rules.ContainsKey(propertyName))
                _rules[propertyName] = new List<ValidationRule>();

            _rules[propertyName].Add(new ValidationRule(rule, errorType, errorMessage));
            return this;
        }

        public List<Error> Validate(object source)
        {
            var values = new Dictionary<string, object>();

            var properties = source.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (_rules.ContainsKey(property.Name))
                {
                    var value = property.GetValue(source) ?? "";
                    values[property.Name] = value;
                }
            }

            return ValidationProcessor.Validate(_rules, values);
        }
    } 
}
