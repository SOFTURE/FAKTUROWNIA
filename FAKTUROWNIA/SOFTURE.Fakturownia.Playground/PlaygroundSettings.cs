using SOFTURE.Fakturownia.Settings;

namespace SOFTURE.Fakturownia.Playground;

internal sealed class PlaygroundSettings : IFakturowniaSettings
{
    public required FakturowniaSettings Fakturownia { get; init; }
}