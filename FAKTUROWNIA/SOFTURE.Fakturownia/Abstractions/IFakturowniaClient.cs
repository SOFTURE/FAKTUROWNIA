using CSharpFunctionalExtensions;
using SOFTURE.Fakturownia.Models.Api.Enums;
using SOFTURE.Fakturownia.Models.Client;

namespace SOFTURE.Fakturownia.Abstractions;

public interface IFakturowniaClient
{
    Task<Result<MonthlyStatement>> GetCurrentMonthStatement(int clientId);
    Task<Result<MonthlyStatement>> GetMonthlyStatement(int clientId, int month, int year);
    Task<Result<Invoice>> GetInvoice(int invoiceId, DocumentKind kind);
    Task<Result<IReadOnlyList<MonthlyStatement>>> GetCurrentlyPaidInvoices(int clientId, List<int> unPayedProInvoiceIds);
    Task<Result<IReadOnlyList<MonthlyStatement>>> GetAllStatements(int clientId);
}