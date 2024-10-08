using Refit;
using SOFTURE.Fakturownia.Models.Api;
using SOFTURE.Fakturownia.Models.Api.Enums;

namespace SOFTURE.Fakturownia.Abstractions;

public interface IFakturowniaApi
{
     [Get("/invoices.json")]
     Task<ApiResponse<IReadOnlyCollection<InvoiceDetails>>> GetInvoicesAsync(
         [AliasAs("period")] Period period, 
         [AliasAs("client_id")] int clientId,
         [Query(CollectionFormat.Multi), AliasAs("kinds[]")] DocumentKind[] kinds);

     [Get("/invoices/{invoiceId}.json")]
     Task<ApiResponse<InvoiceDetails>> GetInvoiceAsync(
         int invoiceId,
         [Query(CollectionFormat.Multi), AliasAs("kinds[]")] DocumentKind[] kinds);
}