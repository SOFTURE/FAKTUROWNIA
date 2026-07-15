using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Mappers;
using SOFTURE.Fakturownia.Models.Api;
using SOFTURE.Fakturownia.Models.Api.Enums;
using SOFTURE.Fakturownia.Models.Client;

namespace SOFTURE.Fakturownia;

internal sealed class FakturowniaClient(IFakturowniaApi fakturowniaApi, ILogger<FakturowniaClient> logger) : IFakturowniaClient
{
    public async Task<Result<MonthlyStatement?>> GetCurrentMonthStatement(int clientId)
    {
        var response = await fakturowniaApi.GetInvoicesAsync(Period.ThisMonth, clientId, [
            DocumentKind.Proforma,
            DocumentKind.Vat
        ]);

        if (!response.IsSuccessStatusCode)
            return Result.Failure<MonthlyStatement?>($"Failed to get invoices for client with id: {clientId} - {response.Error.Content}");

        if (response.Content == null)
            return Result.Failure<MonthlyStatement?>($"Empty response for client with id: {clientId}");

        var invoices = response.Content;

        if (invoices.Count == 0)
            return Result.Success<MonthlyStatement?>(null);

        var proFormaInvoices = invoices
            .Where(i => i.Kind == DocumentKind.Proforma)
            .ToList();

        switch (proFormaInvoices.Count)
        {
            case 0:
                return Result.Success<MonthlyStatement?>(null);
            case > 1:
                return Result.Failure<MonthlyStatement?>(AmbiguousProFormaError(clientId, proFormaInvoices));
        }

        var proFormaInvoice = proFormaInvoices[0];

        var statement = MonthlyStatement.Create(proFormaInvoice.MapToInvoice());

        var invoicesFromProForma = GetInvoicesFromProForma(invoices, proFormaInvoice.Id);

        switch (invoicesFromProForma.Count)
        {
            case > 1:
                return Result.Failure<MonthlyStatement?>(AmbiguousInvoiceError(clientId, proFormaInvoice, invoicesFromProForma));
            case 1:
                statement.Paid(invoicesFromProForma[0].MapToInvoice());
                break;
        }

        return Result.Success<MonthlyStatement?>(statement);
    }

    public async Task<Result<MonthlyStatement>> GetMonthlyStatement(int clientId, int month, int year)
    {
        var response = await fakturowniaApi.GetInvoicesAsync(Period.All, clientId, [
            DocumentKind.Proforma,
            DocumentKind.Vat
        ]);

        if (!response.IsSuccessStatusCode)
            return Result.Failure<MonthlyStatement>($"Failed to get invoices for client with id: {clientId} - {response.Error.Content}");

        if (response.Content == null)
            return Result.Failure<MonthlyStatement>($"Empty response for client with id: {clientId}");

        var allInvoice = response.Content;

        var invoices = allInvoice
            .Where(i => i.IssueDate.Month == month && i.IssueDate.Year == year)
            .ToList();

        if (invoices.Count == 0)
            return Result.Failure<MonthlyStatement>($"Missing invoices for client with id: {clientId}");

        var proFormaInvoices = invoices
            .Where(i => i.Kind == DocumentKind.Proforma)
            .ToList();

        if (proFormaInvoices.Count == 0)
            return Result.Failure<MonthlyStatement>($"Missing proforma invoice for client with id: {clientId}");

        if (proFormaInvoices.Count > 1)
            return Result.Failure<MonthlyStatement>(AmbiguousProFormaError(clientId, proFormaInvoices, month, year));

        var proFormaInvoice = proFormaInvoices[0];

        var statement = MonthlyStatement.Create(
            proFormaInvoice.MapToInvoice()
        );

        var invoicesFromProForma = GetInvoicesFromProForma(allInvoice, proFormaInvoice.Id);

        switch (invoicesFromProForma.Count)
        {
            case > 1:
                return Result.Failure<MonthlyStatement>(AmbiguousInvoiceError(clientId, proFormaInvoice, invoicesFromProForma));
            case 1:
                statement.Paid(invoicesFromProForma[0].MapToInvoice());
                break;
        }

        return Result.Success(statement);
    }

    public async Task<Result<Invoice>> GetInvoice(int invoiceId, DocumentKind kind)
    {
        var response = await fakturowniaApi.GetInvoiceAsync(invoiceId, [kind]);

        if (!response.IsSuccessStatusCode)
            return Result.Failure<Invoice>($"Failed to get invoice with id: {invoiceId} - {response.Error.Content}");

        if (response.Content == null)
            return Result.Failure<Invoice>($"Missing {kind} invoice with id: {invoiceId}");

        var invoice = response.Content;

        return Result.Success(invoice.MapToInvoice());
    }

    public async Task<Result<IReadOnlyList<MonthlyStatement>>> GetCurrentlyPaidInvoices(int clientId, List<int> unPayedProInvoiceIds)
    {
        var response = await fakturowniaApi.GetInvoicesAsync(Period.All, clientId, [
            DocumentKind.Proforma,
            DocumentKind.Vat
        ]);

        if (!response.IsSuccessStatusCode)
            return Result.Failure<IReadOnlyList<MonthlyStatement>>($"Failed to get invoices for client with id: {clientId} - {response.Error.Content}");

        if (response.Content == null)
            return Result.Failure<IReadOnlyList<MonthlyStatement>>($"Empty response for client with id: {clientId}");

        var allInvoice = response.Content;

        var payedInvoices = allInvoice
            .Where(i => i.Kind == DocumentKind.Vat)
            .ToList();

        if (payedInvoices.Count == 0)
            return Result.Success<IReadOnlyList<MonthlyStatement>>(new List<MonthlyStatement>());

        var currentlyPayedStatements = new List<MonthlyStatement>();

        foreach (var unPayedProInvoiceId in unPayedProInvoiceIds)
        {
            var proFormaInvoice = allInvoice
                .SingleOrDefault(i => i.Id == unPayedProInvoiceId && i.Kind == DocumentKind.Proforma);
    
            if (proFormaInvoice == null)
                continue;

            var invoicesFromProForma = payedInvoices
                .Where(i => i.FromInvoiceId == proFormaInvoice.Id)
                .ToList();

            switch (invoicesFromProForma.Count)
            {
                case > 1:
                    return Result.Failure<IReadOnlyList<MonthlyStatement>>(AmbiguousInvoiceError(clientId, proFormaInvoice, invoicesFromProForma));
                case 0:
                    continue;
            }

            var statement = MonthlyStatement.Create(proFormaInvoice.MapToInvoice());

            statement.Paid(invoicesFromProForma[0].MapToInvoice());

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

        if (response.Content == null)
            return Result.Failure<IReadOnlyList<MonthlyStatement>>($"Empty response for client with id: {clientId}");

        var allInvoice = response.Content;

        if (allInvoice.Count == 0)
            return Result.Failure<IReadOnlyList<MonthlyStatement>>($"Missing invoices for client with id: {clientId}");

        var proFormaInvoices = allInvoice
            .Where(i => i.Kind == DocumentKind.Proforma)
            .OrderBy(i => i.IssueDate)
            .ToList();

        var allStatements = new List<MonthlyStatement>();

        foreach (var proFormaInvoice in proFormaInvoices)
        {
            var statement = MonthlyStatement.Create(
                proFormaInvoice.MapToInvoice()
            );

            var invoicesFromProForma = GetInvoicesFromProForma(allInvoice, proFormaInvoice.Id);

            switch (invoicesFromProForma.Count)
            {
                case > 1:
                    allStatements.Add(statement);

                    logger.LogWarning(
                        "Multiple vat invoices ({InvoiceCount}) issued from proforma invoice with id: {ProFormaInvoiceId} " +
                        "({ProFormaIssueDate:yyyy-MM-dd}) for client with id: {ClientId} - vat invoice ids: {InvoiceIds}",
                        invoicesFromProForma.Count,
                        proFormaInvoice.Id,
                        proFormaInvoice.IssueDate,
                        clientId,
                        FormatIds(invoicesFromProForma
                        )
                    );

                    continue;
                case 1:
                    statement.Paid(invoicesFromProForma[0].MapToInvoice());
                    break;
            }

            allStatements.Add(statement);
        }

        return Result.Success<IReadOnlyList<MonthlyStatement>>(allStatements);
    }

    private static List<InvoiceDetails> GetInvoicesFromProForma(IEnumerable<InvoiceDetails> invoices, int proFormaInvoiceId)
    {
        return invoices
            .Where(i => i.Kind == DocumentKind.Vat && i.FromInvoiceId == proFormaInvoiceId)
            .ToList();
    }

    private static string AmbiguousProFormaError(
        int clientId,
        IReadOnlyCollection<InvoiceDetails> proFormaInvoices,
        int? month = null,
        int? year = null)
    {
        var period = month.HasValue && year.HasValue
            ? $"{month:D2}/{year}"
            : "current month";

        return $"Multiple proforma invoices ({proFormaInvoices.Count}) for client with id: {clientId} " +
               $"in {period} - proforma invoice ids: {FormatIds(proFormaInvoices)}";
    }

    private static string AmbiguousInvoiceError(int clientId, InvoiceDetails proFormaInvoice, IReadOnlyCollection<InvoiceDetails> invoices)
    {
        return $"Multiple vat invoices ({invoices.Count}) issued from proforma invoice with id: {proFormaInvoice.Id} " +
               $"for client with id: {clientId} - vat invoice ids: {FormatIds(invoices)}";
    }

    private static string FormatIds(IEnumerable<InvoiceDetails> invoices)
    {
        return string.Join(", ", invoices.Select(i => i.Id));
    }
}
