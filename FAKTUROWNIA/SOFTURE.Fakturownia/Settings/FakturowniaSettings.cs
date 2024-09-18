namespace SOFTURE.Fakturownia.Settings;

public sealed class FakturowniaSettings
{
#if NET8_0
    public required string Url { get; init; }
    public required string ApiKey { get; init; }
#endif

#if NET6_0
    public string Url { get; init; } = null!;
    public string ApiKey { get; init; } = null!;
#endif
}