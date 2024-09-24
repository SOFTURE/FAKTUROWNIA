using CSharpFunctionalExtensions;
using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Models.Client;
using SOFTURE.Fakturownia.Models.Enums;

namespace SOFTURE.Fakturownia;

internal sealed class FakturowniaClient(IFakturowniaApi fakturowniaApi) : IFakturowniaClient
{
    public async Task<Result<CurrentMonthStatement>> GetCurrentMonthStatement(int clientId)
    {
        var response = await fakturowniaApi.GetInvoicesAsync(Period.ThisMonth, clientId);

        if (!response.IsSuccessStatusCode)
            return Result.Failure<CurrentMonthStatement>(
                $"Failed to get invoices for client with id: {clientId} - {response.Error.Content}"
            );
        
        var invoices = response.Content;

        if (invoices.Count == 0)
            return Result.Failure<CurrentMonthStatement>($"Missing invoices for client with id: {clientId}");

        var proFormaInvoice = invoices.SingleOrDefault(i => i.Kind == DocumentKind.Proforma);

        if (proFormaInvoice == null)
            return Result.Failure<CurrentMonthStatement>($"Missing proforma invoice for client with id: {clientId}");

        var statement = CurrentMonthStatement.Create(
            proFormaInvoice.PriceNet!,
            new Invoice(proFormaInvoice.Id, proFormaInvoice.CreatedAt)
        );

        var invoice = invoices.SingleOrDefault(i => i.Kind == DocumentKind.Vat &&
                                                    i.FromInvoiceId == proFormaInvoice.Id);
        
        if (invoice != null)
            statement.Paid(new Invoice(invoice.Id, invoice.CreatedAt));

        return Result.Success(statement);
    }
}