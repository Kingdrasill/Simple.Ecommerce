namespace Simple.Ecommerce.Contracts.CacheFrequencyContracts
{
    public record CacheFrequencyResponse
    (
        int Id,
        string Entity,
        int Frequency,
        int? HoursToLive,
        bool Expirable,
        bool KeepCached
    );
}
