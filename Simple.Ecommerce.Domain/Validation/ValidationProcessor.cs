using Simple.Ecommerce.Domain.Errors.BaseError;

namespace Simple.Ecommerce.Domain.Validation
{
    public static class ValidationProcessor
    {
        public static List<Error> Validate(Dictionary<string, List<ValidationRule>> rules, Dictionary<string, object> values)
        {
            var errors = new List<Error>();

            foreach (var (propertyName, value) in values)
            {
                if (!rules.TryGetValue(propertyName, out var propertyRules))
                    continue;

                foreach (var rule in propertyRules)
                {
                    if (rule.Rule(value))
                    {
                        errors.Add(new Error($"{rule.ErrorType}.{propertyName}", rule.ErrorMessage));
                    }
                }
            }

            return errors;
        }
    }
}
