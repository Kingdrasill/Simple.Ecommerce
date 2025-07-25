using Simple.Ecommerce.Domain.Errors.BaseError;

namespace Simple.Ecommerce.Domain.Validation
{
    public static class ValidationProcessor
    {
        public static List<Error> Validate(
            Dictionary<string, List<ValidationRule>> rules, 
            Dictionary<string, object> values, 
            Dictionary<string, List<ValidationRule>> entityRules,
            object entity
        )
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

            foreach (var (propName, propRules) in entityRules)
            {
                foreach (var rule in propRules)
                {
                    if (rule.Rule(entity))
                    {
                        errors.Add(new Error($"{rule.ErrorType}.{propName}", rule.ErrorMessage));
                    }
                }
            }

            return errors;
        }
    }
}
