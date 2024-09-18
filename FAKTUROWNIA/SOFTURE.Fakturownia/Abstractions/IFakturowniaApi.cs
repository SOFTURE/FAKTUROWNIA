using Refit;
using SOFTURE.Fakturownia.Models;
using SOFTURE.Fakturownia.Models.Enums;

namespace SOFTURE.Fakturownia.Abstractions;

public interface IFakturowniaApi
{
     [Get("/invoices.json")]
     Task<IReadOnlyCollection<InvoiceDetails>> GetInvoicesAsync(
         [AliasAs("period")] Period period, 
         [AliasAs("client_id")] int clientId);
}