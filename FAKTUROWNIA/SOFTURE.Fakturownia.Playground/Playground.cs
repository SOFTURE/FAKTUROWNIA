using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Models.Enums;

namespace SOFTURE.Fakturownia.Playground;

public sealed class Playground(IFakturowniaClient fakturowniaClient)
{
    public async Task Run()
    {
        var currentMonthStatement = await fakturowniaClient.GetCurrentMonthStatement(clientId: 135057762);

        var monthlyStatement = await fakturowniaClient.GetMonthlyStatement(
            clientId: 135057762,
            month: 7,
            year: 2024
        );
        
        var invoice = await fakturowniaClient.GetInvoice(invoiceId: 315738314, kind: DocumentKind.Proforma);
    }
}