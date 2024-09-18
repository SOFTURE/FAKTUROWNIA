
using SOFTURE.Fakturownia.Abstractions;

namespace SOFTURE.Fakturownia.Playground;

public sealed class Playground(IFakturowniaClient fakturowniaClient)
{
    public async Task Run()
    {
        var currentMonthStatement = await fakturowniaClient.GetCurrentMonthStatement(clientId: 116360544);
    }
}