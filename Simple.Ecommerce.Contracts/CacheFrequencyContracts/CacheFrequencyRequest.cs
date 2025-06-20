namespace Simple.Ecommerce.Contracts.CacheFrequencyContracts
{
    public record CacheFrequencyRequest
    (
        string Entity,
        int Frequency,
        int? HoursToLive,
        bool Expirable,
        bool KeepCached,
        int Id = 0
    );
}
