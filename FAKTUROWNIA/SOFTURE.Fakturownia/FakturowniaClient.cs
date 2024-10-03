using CSharpFunctionalExtensions;
using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Models.Api.Enums;
using SOFTURE.Fakturownia.Models.Client;

namespace SOFTURE.Fakturownia;

internal sealed class FakturowniaClient(IFakturowniaApi fakturowniaApi) : IFakturowniaClient
{
    public async Task<Result<MonthlyStatement>> GetCurrentMonthStatement(int clientId)
    {
        var response = await fakturowniaApi.GetInvoicesAsync(Period.ThisMonth, clientId, [
            DocumentKind.Proforma,
            DocumentKind.Vat
        ]);

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
            new Invoice(proFormaInvoice.Id, proFormaInvoice.IssueDate, proFormaInvoice.PriceNet)
        );

        var invoice = invoices.SingleOrDefault(i => i.Kind == DocumentKind.Vat &&
                                                    i.FromInvoiceId == proFormaInvoice.Id);

        if (invoice != null)
            statement.Paid(new Invoice(invoice.Id, invoice.IssueDate, invoice.PriceNet));

        return Result.Success(statement);
    }

    public async Task<Result<MonthlyStatement>> GetMonthlyStatement(int clientId, int month, int year)
    {
        var response = await fakturowniaApi.GetInvoicesAsync(Period.All, clientId, [
            DocumentKind.Proforma,
            DocumentKind.Vat
        ]);
    
        if (!response.IsSuccessStatusCode)
            return Result.Failure<MonthlyStatement>(
                $"Failed to get invoices for client with id: {clientId} - {response.Error.Content}"
            );
    
        var allInvoice = response.Content;
    
        var invoices = allInvoice
            .Where(i => i.IssueDate.Month == month && i.IssueDate.Year == year)
            .ToList();
    
        if (invoices.Count == 0)
            return Result.Failure<MonthlyStatement>($"Missing invoices for client with id: {clientId}");
    
        var proFormaInvoice = invoices.SingleOrDefault(i => i.Kind == DocumentKind.Proforma);
    
        if (proFormaInvoice == null)
            return Result.Failure<MonthlyStatement>($"Missing proforma invoice for client with id: {clientId}");
    
        var statement = MonthlyStatement.Create(
            new Invoice(proFormaInvoice.Id, proFormaInvoice.IssueDate, proFormaInvoice.PriceNet)
        );
    
        var invoice = allInvoice.SingleOrDefault(i => i.Kind == DocumentKind.Vat &&
                                                      i.FromInvoiceId == proFormaInvoice.Id);
    
        if (invoice != null)
            statement.Paid(new Invoice(invoice.Id, invoice.IssueDate, invoice.PriceNet));
    
        return Result.Success(statement);
    }
    
    public async Task<Result<Invoice>> GetInvoice(int invoiceId, DocumentKind kind)
    {
        var response = await fakturowniaApi.GetInvoiceAsync(invoiceId, [kind]);
    
        if (!response.IsSuccessStatusCode)
            return Result.Failure<Invoice>(
                $"Failed to get invoice with id: {invoiceId} - {response.Error.Content}"
            );
    
        if (response.Content == null)
            return Result.Failure<Invoice>($"Missing {kind} invoice with id: {invoiceId}");
    
        var invoice = response.Content;

        return Result.Success(new Invoice(invoice.Id, invoice.IssueDate, invoice.PriceNet));
    }
    
    public async Task<Result<IReadOnlyList<MonthlyStatement>>> GetCurrentlyPaidInvoices(
        int clientId,
        List<int> unPayedProInvoiceIds)
    {
        var response = await fakturowniaApi.GetInvoicesAsync(Period.All, clientId, [
            DocumentKind.Proforma,
            DocumentKind.Vat
        ]);
    
        if (!response.IsSuccessStatusCode)
            return Result.Failure<IReadOnlyList<MonthlyStatement>>(
                $"Failed to get invoices for client with id: {clientId} - {response.Error.Content}"
            );
    
        var allInvoice = response.Content;
    
        var payedInvoices = allInvoice
            .Where(i => i.Kind == DocumentKind.Vat)
            .ToList();
    
        if (payedInvoices.Count == 0)
            return Result.Failure<IReadOnlyList<MonthlyStatement>>(
                $"Missing payed invoices for client with id: {clientId}");
    
        var currentlyPayedStatements = new List<MonthlyStatement>();
    
        foreach (var unPayedProInvoiceId in unPayedProInvoiceIds)
        {
            var proFormaInvoice = allInvoice
                .SingleOrDefault(i => i.Id == unPayedProInvoiceId && i.Kind == DocumentKind.Proforma);
    
            if (proFormaInvoice == null)
                continue;
    
            var invoice = payedInvoices.SingleOrDefault(i => i.FromInvoiceId == proFormaInvoice.Id);
    
            if (invoice == null)
                continue;
    
            var statement = MonthlyStatement.Create(
                new Invoice(proFormaInvoice.Id, proFormaInvoice.IssueDate, proFormaInvoice.PriceNet)
            );
    
            statement.Paid(new Invoice(invoice.Id, invoice.IssueDate, invoice.PriceNet));
    
            currentlyPayedStatements.Add(statement);
        }
    
        return Result.Success<IReadOnlyList<MonthlyStatement>>(currentlyPayedStatements);
    }
    
    public async Task<Result<IReadOnlyList<MonthlyStatement>>> GetAllStatements(int clientId)
    {
        var response = await fakturowniaApi.GetInvoicesAsync(Period.All, clientId, [
            DocumentKind.Proforma,
            DocumentKind.Vat
        ]);
    
        if (!response.IsSuccessStatusCode)
            return Result.Failure<IReadOnlyList<MonthlyStatement>>(
                $"Failed to get invoices for client with id: {clientId} - {response.Error.Content}"
            );
    
        var allInvoice = response.Content;
    
        if (allInvoice.Count == 0)
            return Result.Failure<IReadOnlyList<MonthlyStatement>>($"Missing invoices for client with id: {clientId}");
    
        var proFormaInvoices = allInvoice
            .Where(i => i.Kind == DocumentKind.Proforma)
            .OrderBy(i => i.IssueDate)
            .ToList();
    
        if (allInvoice.Count == 0)
            return Result.Failure<IReadOnlyList<MonthlyStatement>>(
                $"Missing payed invoices for client with id: {clientId}");
    
        var allStatements = new List<MonthlyStatement>();
    
        foreach (var proFormaInvoice in proFormaInvoices)
        {
            var statement = MonthlyStatement.Create(
                new Invoice(proFormaInvoice.Id, proFormaInvoice.IssueDate, proFormaInvoice.PriceNet)
            );
    
            var invoiceCount = allInvoice.Count(i => i.FromInvoiceId == proFormaInvoice.Id &&
                                                     i.Kind == DocumentKind.Vat);
    
            if (invoiceCount > 1)
            {
                allStatements.Add(statement);
    
                Console.WriteLine(
                    $"ClientId: {clientId} - Multiple invoices for PRO-FV {proFormaInvoice.Id} - {proFormaInvoice.IssueDate}");
    
                continue;
            }
    
            var invoice = allInvoice.SingleOrDefault(i => i.FromInvoiceId == proFormaInvoice.Id &&
                                                          i.Kind == DocumentKind.Vat);
    
            if (invoice == null)
            {
                allStatements.Add(statement);
                continue;
            }
    
            statement.Paid(new Invoice(invoice.Id, invoice.IssueDate, invoice.PriceNet));
    
            allStatements.Add(statement);
        }
    
        return Result.Success<IReadOnlyList<MonthlyStatement>>(allStatements);
    }
}