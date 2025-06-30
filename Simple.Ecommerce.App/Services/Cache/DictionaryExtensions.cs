using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;

namespace Simple.Ecommerce.App.Services.Cache
{
    public static class DictionaryExtensions
    {
        public static int? GetNullableInt(this IDictionary<string, object> dict, string key)
        {
            return dict.TryGetValue(key, out var value) && value is not null ? Convert.ToInt32(value) : null;
        }

        public static string? GetNullableString(this IDictionary<string, object> dict, string key)
        {
            return dict.TryGetValue(key, out var value) && value is not null ? value.ToString() : null;
        }

        public static DateTime? GetNullableDateTime(this IDictionary<string, object> dict, string key)
        {
            return dict.TryGetValue(key, out var value) && value is not null ? Convert.ToDateTime(value) : null;
        }

        public static decimal? GetNullableDecimal(this IDictionary<string, object> dict, string key)
        {
            return dict.TryGetValue(key, out var value) && value is not null ? Convert.ToDecimal(value) : null;
        }

        public static DiscountValueType? GetNullableDiscountValueType(this IDictionary<string, object> dict, string key)
        {
            return dict.TryGetValue(key, out var value) && value is not null ? (DiscountValueType)Convert.ToInt32(value) : null;
        }

        public static PaymentMethod? GetNullablePaymentMethod(this IDictionary<string, object> dict, string key)
        {
            return dict.TryGetValue(key, out var value) && value is not null ? (PaymentMethod)Convert.ToInt32(value) : null;
        }

        public static bool GetBoolean(this IDictionary<string, object> dict, string key)
        {
            if (!dict.ContainsKey(key) || dict[key] is null) 
                return false;

            var value = dict[key]?.ToString()?.ToLowerInvariant();
            return value == "true" || value == "1";
        }
    }
}
