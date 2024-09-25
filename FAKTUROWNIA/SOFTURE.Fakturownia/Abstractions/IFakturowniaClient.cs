using CSharpFunctionalExtensions;
using SOFTURE.Fakturownia.Models;
using SOFTURE.Fakturownia.Models.Client;
using SOFTURE.Fakturownia.Models.Enums;

namespace SOFTURE.Fakturownia.Abstractions;

public interface IFakturowniaClient
{
    Task<Result<MonthlyStatement>> GetCurrentMonthStatement(int clientId);
    Task<Result<MonthlyStatement>> GetMonthlyStatement(int clientId, int month, int year);
    Task<Result<Invoice>> GetInvoice(int invoiceId, DocumentKind kind);
}