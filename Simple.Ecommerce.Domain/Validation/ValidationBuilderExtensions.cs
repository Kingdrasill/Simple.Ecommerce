namespace Simple.Ecommerce.Domain.Validation
{
    public static class ValidationBuilderExtensions
    {
        public static ValidationBuilder AddNegativeValueDecimal(this ValidationBuilder builder, string propertyName, string className)
        {
            builder.AddRule(propertyName, n => (decimal)n < 0, $"{className}.NegativeValue", "Não pode ser negativo!");
            return builder;
        }

        public static ValidationBuilder AddNegativeValueInt(this ValidationBuilder builder, string propertyName, string className)
        {
            builder.AddRule(propertyName, n => (int)n < 0, $"{className}.NegativeValue", "Não pode ser negativo!");
            return builder;
        }

        public static ValidationBuilder AddOutOfRange(this ValidationBuilder builder, string propertyName, string className, int minNumber, int maxNumber)
        {
            builder.AddRule(propertyName, n => (int)n < minNumber || (int)n > maxNumber, $"{className}.OutOfRange", $"Precisa estar entre {minNumber} e {maxNumber}!");
            return builder;
        }

        public static ValidationBuilder AddEmptyValue(this ValidationBuilder builder, string propertyName, string className)
        {
            builder.AddRule(propertyName, s => String.IsNullOrEmpty((string)s), $"{className}.EmptyValue", "Não pode ser vazio ou nulo!");
            return builder;
        }

        public static ValidationBuilder AddMaxLength(this ValidationBuilder builder, string propertyName, string className, int maxLength)
        {
            builder.AddRule(propertyName, s => ((string)s).Length > maxLength, $"{className}.MaxLength", $"Não pode ser ter mais de {maxLength} caracteres!");
            return builder;
        }

        public static ValidationBuilder AddMinLength(this ValidationBuilder builder, string propertyName, string className, int minLength)
        {
            builder.AddRule(propertyName, s => ((string)s).Length < minLength, $"{className}.MaxLength", $"Não pode ser ter menos de {minLength} caracteres!");
            return builder;
        }

        public static ValidationBuilder AddInvalidDateRange<T>(this ValidationBuilder builder, string propertyName, string className, Func<T, DateTime?> startSelector, Func<T, DateTime?> endSelector)
        {
            builder.AddRule(
                propertyName,
                e =>
                {
                    var start = startSelector((T)e);
                    var end = endSelector((T)e);
                    if (start == null && end == null) return false;
                    if (start == null || end == null) return true;
                    return end <= start;
                },
                $"{className}.InvalidDateRange",
                "Ambas as datas devem ser vazias, ou a de término deve ser posterior à de começo!"
            );
            return builder;
        }
    }
}
