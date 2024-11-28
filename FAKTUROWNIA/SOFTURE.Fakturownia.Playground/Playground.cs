using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Models.Api.Enums;

namespace SOFTURE.Fakturownia.Playground;

public sealed class Playground(IFakturowniaClient fakturowniaClient)
{
    public async Task Run()
    {
        // var currentMonthStatement = await fakturowniaClient.GetCurrentMonthStatement(clientId: 145264330);
     
        // var monthlyStatement = await fakturowniaClient.GetMonthlyStatement(
        //     clientId: 135057762,
        //     month: 7,
        //     year: 2024
        // );
        
        var invoiceProforma = await fakturowniaClient.GetInvoice(invoiceId: 294812795, kind: DocumentKind.Proforma);
        var invoiceVat = await fakturowniaClient.GetInvoice(invoiceId: 300462117, kind: DocumentKind.Vat);
        
        // var currentlyPaidInvoices = await fakturowniaClient.GetCurrentlyPaidInvoices(
        //     clientId: 135057762,
        //     unPayedProInvoiceIds: [294812716, 315738315]
        // );
        
        // var allStatements = await fakturowniaClient.GetAllStatements(clientId: 145264330);
    }
}