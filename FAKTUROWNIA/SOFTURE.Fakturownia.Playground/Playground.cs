using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Models.Api.Enums;

namespace SOFTURE.Fakturownia.Playground;

public sealed class Playground(IFakturowniaClient fakturowniaClient)
{
    public async Task Run()
    {
        //var currentMonthStatement = await fakturowniaClient.GetCurrentMonthStatement(clientId: 145264330);
     
        // var monthlyStatement = await fakturowniaClient.GetMonthlyStatement(
        //     clientId: 135057762,
        //     month: 7,
        //     year: 2024
        // );
        
        // var invoice = await fakturowniaClient.GetInvoice(invoiceId: 315738314, kind: DocumentKind.Proforma);
        
        // var currentlyPaidInvoices = await fakturowniaClient.GetCurrentlyPaidInvoices(
        //     clientId: 135057762,
        //     unPayedProInvoiceIds: [294812716, 315738315]
        // );
        
        var allStatements = await fakturowniaClient.GetAllStatements(clientId: 149831120);
    }
}