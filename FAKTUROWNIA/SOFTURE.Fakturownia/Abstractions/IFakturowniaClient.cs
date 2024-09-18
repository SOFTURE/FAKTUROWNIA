using CSharpFunctionalExtensions;
using SOFTURE.Fakturownia.Models;
using SOFTURE.Fakturownia.Models.Client;

namespace SOFTURE.Fakturownia.Abstractions;

public interface IFakturowniaClient
{
    Task<Result<CurrentMonthStatement>> GetCurrentMonthStatement(int clientId);
}