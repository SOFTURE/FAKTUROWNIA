# SOFTURE.Fakturownia

.NET client library for the [Fakturownia](https://fakturownia.pl) invoicing API.

[![NuGet](https://img.shields.io/nuget/v/SOFTURE.Fakturownia)](https://www.nuget.org/packages/SOFTURE.Fakturownia)

**Target Frameworks:** `net6.0`, `net8.0`

## Installation

```bash
dotnet add package SOFTURE.Fakturownia
```

## Configuration

Add Fakturownia settings to your `appsettings.json`:

```json
{
  "Fakturownia": {
    "Url": "https://app.fakturownia.pl",
    "ApiKey": "your-api-key"
  }
}
```

| Property | Description |
|----------|-------------|
| `Url` | Fakturownia API base URL |
| `ApiKey` | API token from your Fakturownia account |

Implement `IFakturowniaSettings` in your settings class:

```csharp
using SOFTURE.Fakturownia.Settings;

public class AppSettings : IFakturowniaSettings
{
    public FakturowniaSettings Fakturownia { get; init; }
}
```

## Registration

Register services in your DI container:

```csharp
services.AddFakturownia<AppSettings>();
```

This registers:
- `IFakturowniaClient` — main client (scoped)
- Refit HTTP client with automatic API key injection
- Polly retry policy — 5 retries with exponential backoff (handles transient errors and `429 TooManyRequests`)
- Health check (`FakturowniaHealthCheck`)

## Usage

Inject `IFakturowniaClient` and call methods:

```csharp
using SOFTURE.Fakturownia.Abstractions;

public class InvoiceService
{
    private readonly IFakturowniaClient _client;

    public InvoiceService(IFakturowniaClient client)
    {
        _client = client;
    }

    public async Task Example()
    {
        var statement = await _client.GetCurrentMonthStatement(clientId: 123);
        var allStatements = await _client.GetAllStatements(clientId: 123);
    }
}
```

### Available Methods

| Method | Description |
|--------|-------------|
| `GetCurrentMonthStatement(int clientId)` | Get proforma/VAT invoice pair for current month |
| `GetMonthlyStatement(int clientId, int month, int year)` | Get proforma/VAT invoice pair for a specific month |
| `GetInvoice(int invoiceId, DocumentKind kind)` | Get a single invoice by ID and document type |
| `GetCurrentlyPaidInvoices(int clientId, List<int> unPayedProInvoiceIds)` | Check which proforma invoices have been paid |
| `GetAllStatements(int clientId)` | Get all monthly statements for a client |

All methods return `Task<Result<T>>` using [CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions).

## Models

### Invoice

| Property | Type | Description |
|----------|------|-------------|
| `Identifier` | `int` | Invoice ID |
| `Number` | `string` | Invoice number |
| `IssueDate` | `DateTime` | Issue date |
| `PaymentDate` | `DateTime` | Payment due date |
| `PriceNet` | `decimal` | Net price |

### MonthlyStatement

| Property | Type | Description |
|----------|------|-------------|
| `ProFormaInvoice` | `Invoice` | Proforma invoice |
| `Invoice` | `Invoice?` | VAT invoice (null if unpaid) |
| `IsPaid` | `bool` | Whether the proforma has a matching VAT invoice |

### DocumentKind

`Vat`, `Proforma`, `Bill`, `Receipt`, `Advance`, `Correction`, `VatMp`, `InvoiceOther`, `VatMargin`, `Kp`, `Kw`, `Final`, `Estimate`, `AccountingNote`

## License

See [LICENSE](LICENSE) file.
