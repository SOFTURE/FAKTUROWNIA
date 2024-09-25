using CSharpFunctionalExtensions;
using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Models.Client;
using SOFTURE.Fakturownia.Models.Enums;

namespace SOFTURE.Fakturownia;

internal sealed class FakturowniaClient(IFakturowniaApi fakturowniaApi) : IFakturowniaClient
{
    public async Task<Result<MonthlyStatement>> GetCurrentMonthStatement(int clientId)
    {
        var response = await fakturowniaApi.GetInvoicesAsync(Period.ThisMonth, clientId);

        if (!response.IsSuccessStatusCode)
            return Result.Failure<MonthlyStatement>(
                $"Failed to get invoices for client with id: {clientId} - {response.Error.Content}"
            );
        
        var invoices = response.Content;

        if (invoices.Count == 0)
            return Result.Failure<MonthlyStatement>($"Missing invoices for client with id: {clientId}");

        var proFormaInvoice = invoices.SingleOrDefault(i => i.Kind == DocumentKind.Proforma);

        if (proFormaInvoice == null)
            return Result.Failure<MonthlyStatement>($"Missing proforma invoice for client with id: {clientId}");

        var statement = MonthlyStatement.Create(
            proFormaInvoice.PriceNet!,
            new Invoice(proFormaInvoice.Id, proFormaInvoice.CreatedAt)
        );

        var invoice = invoices.SingleOrDefault(i => i.Kind == DocumentKind.Vat &&
                                                    i.FromInvoiceId == proFormaInvoice.Id);
        
        if (invoice != null)
            statement.Paid(new Invoice(invoice.Id, invoice.CreatedAt));

        return Result.Success(statement);
    }
    
    public async Task<Result<MonthlyStatement>> GetMonthlyStatement(int clientId, int month, int year)
    {
        var response = await fakturowniaApi.GetInvoicesAsync(Period.All, clientId);

        if (!response.IsSuccessStatusCode)
            return Result.Failure<MonthlyStatement>(
                $"Failed to get invoices for client with id: {clientId} - {response.Error.Content}"
            );
        
        var allInvoice = response.Content;
        
        var invoices = allInvoice
            .Where(i => i.CreatedAt.Month == month && i.CreatedAt.Year == year)
            .ToList();

        if (invoices.Count == 0)
            return Result.Failure<MonthlyStatement>($"Missing invoices for client with id: {clientId}");

        var proFormaInvoice = invoices.SingleOrDefault(i => i.Kind == DocumentKind.Proforma);

        if (proFormaInvoice == null)
            return Result.Failure<MonthlyStatement>($"Missing proforma invoice for client with id: {clientId}");

        var statement = MonthlyStatement.Create(
            proFormaInvoice.PriceNet!,
            new Invoice(proFormaInvoice.Id, proFormaInvoice.CreatedAt)
        );

        var invoice = allInvoice.SingleOrDefault(i => i.Kind == DocumentKind.Vat &&
                                                      i.FromInvoiceId == proFormaInvoice.Id);
        
        if (invoice != null)
            statement.Paid(new Invoice(invoice.Id, invoice.CreatedAt));

        return Result.Success(statement);
    }
}