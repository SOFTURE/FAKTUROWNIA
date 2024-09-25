using Refit;
using SOFTURE.Fakturownia.Models;
using SOFTURE.Fakturownia.Models.Enums;

namespace SOFTURE.Fakturownia.Abstractions;

public interface IFakturowniaApi
{
     [Get("/invoices.json")]
     Task<ApiResponse<IReadOnlyCollection<InvoiceDetails>>> GetInvoicesAsync(
         [AliasAs("period")] Period period, 
         [AliasAs("client_id")] int clientId);

     [Get("/invoices/{invoiceId}.json")]
     Task<ApiResponse<InvoiceDetails>> GetInvoiceAsync(int invoiceId);
}