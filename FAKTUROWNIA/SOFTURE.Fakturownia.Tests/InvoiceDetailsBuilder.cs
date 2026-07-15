using System.Net;
using Refit;
using SOFTURE.Fakturownia.Models.Api;
using SOFTURE.Fakturownia.Models.Api.Enums;

namespace SOFTURE.Fakturownia.Tests;

internal static class InvoiceDetailsBuilder
{
    public static InvoiceDetails ProForma(int id, string priceNet = "708.00", DateTime? issueDate = null)
    {
        return Create(id, DocumentKind.Proforma, priceNet, issueDate);
    }

    public static InvoiceDetails Vat(int id, int fromProFormaId, string priceNet = "708.00", DateTime? issueDate = null)
    {
        var invoice = Create(id, DocumentKind.Vat, priceNet, issueDate);

        invoice.FromInvoiceId = fromProFormaId;

        return invoice;
    }

    public static ApiResponse<IReadOnlyCollection<InvoiceDetails>> AsApiResponse(
        this IReadOnlyCollection<InvoiceDetails> invoices)
    {
        return new ApiResponse<IReadOnlyCollection<InvoiceDetails>>(
            new HttpResponseMessage(HttpStatusCode.OK),
            invoices,
            new RefitSettings()
        );
    }

    private static InvoiceDetails Create(int id, DocumentKind kind, string priceNet, DateTime? issueDate)
    {
        var date = issueDate ?? new DateTime(2026, 6, 1);

        return new InvoiceDetails
        {
            Id = id,
            Kind = kind,
            Number = $"{kind}/{id}",
            PriceNet = priceNet,
            IssueDate = date,
            PaymentTo = date.AddDays(14)
        };
    }
}
