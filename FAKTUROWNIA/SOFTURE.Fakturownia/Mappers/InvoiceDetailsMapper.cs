using SOFTURE.Fakturownia.Models.Api;
using SOFTURE.Fakturownia.Models.Client;

namespace SOFTURE.Fakturownia.Mappers;

internal static class InvoiceDetailsMapper
{
    public static Invoice MapToInvoice(this InvoiceDetails invoiceDetails)
    {
        return new Invoice(
            identifier: invoiceDetails.Id,
            number: invoiceDetails.Number,
            issueDate: invoiceDetails.IssueDate,
            paymentDate: invoiceDetails.PaymentTo,
            priceNet: invoiceDetails.PriceNet
        );
    }
}