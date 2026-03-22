# SOFTURE.Fakturownia

A .NET client library for the [Fakturownia](https://fakturownia.pl) invoicing API — providing a strongly-typed, resilient interface for managing invoices and monthly statements.

[![NuGet](https://img.shields.io/nuget/v/SOFTURE.Fakturownia)](https://www.nuget.org/packages/SOFTURE.Fakturownia)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%208.0-blue)](https://dotnet.microsoft.com/)

---

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Configuration](#configuration)
- [Service Registration](#service-registration)
- [Usage](#usage)
- [Models](#models)
- [Error Handling](#error-handling)
- [Health Checks](#health-checks)
- [Architecture](#architecture)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)

---

## Features

- Strongly-typed client for the Fakturownia REST API
- Built-in retry policy with exponential backoff (Polly) — handles transient errors and `429 TooManyRequests`
- Automatic API key injection via custom `DelegatingHandler`
- Functional error handling with `Result<T>` ([CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions))
- Monthly statement aggregation — pairs proforma and VAT invoices with payment status tracking
- Health check integration for monitoring API availability
- Supports 14 document types (VAT, Proforma, Bill, Receipt, and more)
- Multi-target framework support: `.NET 6.0` and `.NET 8.0`
- Clean dependency injection registration via a single extension method

---

## Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A [Fakturownia](https://fakturownia.pl) account with an API token

---

## Installation

### .NET CLI

```bash
dotnet add package SOFTURE.Fakturownia
```

### Package Manager

```powershell
Install-Package SOFTURE.Fakturownia
```

### PackageReference

```xml
<PackageReference Include="SOFTURE.Fakturownia" Version="*" />
```

---

## Quick Start

```csharp
// 1. Implement the settings interface
public class AppSettings : IFakturowniaSettings
{
    public FakturowniaSettings Fakturownia { get; init; }
}

// 2. Register services
services.AddFakturownia<AppSettings>();

// 3. Inject and use
public class InvoiceService(IFakturowniaClient client)
{
    public async Task PrintCurrentStatement(int clientId)
    {
        var result = await client.GetCurrentMonthStatement(clientId);

        if (result.IsSuccess)
        {
            var statement = result.Value;
            Console.WriteLine($"Proforma: {statement.ProFormaInvoice.Number}");
            Console.WriteLine($"Paid: {statement.IsPaid}");
        }
    }
}
```

---

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

| Property | Type     | Description                              |
|----------|----------|------------------------------------------|
| `Url`    | `string` | Fakturownia API base URL                 |
| `ApiKey` | `string` | API token from your Fakturownia account  |

Create a settings class that implements `IFakturowniaSettings`:

```csharp
using SOFTURE.Fakturownia.Settings;

public class AppSettings : IFakturowniaSettings
{
    public FakturowniaSettings Fakturownia { get; init; }
}
```

---

## Service Registration

Register all Fakturownia services with a single call:

```csharp
services.AddFakturownia<AppSettings>();
```

This registers:

| Service                    | Lifetime | Description                                                     |
|----------------------------|----------|-----------------------------------------------------------------|
| `IFakturowniaClient`      | Scoped   | Main client for interacting with the Fakturownia API            |
| `IFakturowniaApi`         | —        | Refit HTTP client with automatic API key injection              |
| `GetApiTokenHandler`      | —        | `DelegatingHandler` that appends the API token to each request  |
| `FakturowniaHealthCheck`  | —        | Health check for monitoring API availability                    |
| Polly retry policy         | —        | 5 retries with exponential backoff for transient errors         |

---

## Usage

Inject `IFakturowniaClient` into your services and use the available methods.

### Get Current Month Statement

Retrieves the proforma/VAT invoice pair for the current month.

```csharp
var result = await client.GetCurrentMonthStatement(clientId: 123);

if (result.IsSuccess)
{
    var statement = result.Value;
    Console.WriteLine($"Proforma: {statement.ProFormaInvoice.Number}");
    Console.WriteLine($"VAT Invoice: {statement.Invoice?.Number ?? "Not yet paid"}");
}
```

### Get Monthly Statement

Retrieves the proforma/VAT invoice pair for a specific month.

```csharp
var result = await client.GetMonthlyStatement(clientId: 123, month: 3, year: 2024);
```

### Get Single Invoice

Retrieves a single invoice by ID and document type.

```csharp
var result = await client.GetInvoice(invoiceId: 294812795, kind: DocumentKind.Proforma);

if (result.IsSuccess)
{
    var invoice = result.Value;
    Console.WriteLine($"{invoice.Number} — {invoice.PriceNet:C}");
}
```

### Get Currently Paid Invoices

Checks which proforma invoices have been paid (matched with a VAT invoice).

```csharp
var unpaidIds = new List<int> { 100, 200, 300 };
var result = await client.GetCurrentlyPaidInvoices(clientId: 123, unPayedProInvoiceIds: unpaidIds);

if (result.IsSuccess)
{
    foreach (var statement in result.Value)
    {
        Console.WriteLine($"{statement.ProFormaInvoice.Number} — Paid: {statement.IsPaid}");
    }
}
```

### Get All Statements

Retrieves all monthly statements for a given client.

```csharp
var result = await client.GetAllStatements(clientId: 123);

if (result.IsSuccess)
{
    foreach (var statement in result.Value)
    {
        Console.WriteLine($"{statement.ProFormaInvoice.Number} — Paid: {statement.IsPaid}");
    }
}
```

### API Reference

| Method | Returns | Description |
|--------|---------|-------------|
| `GetCurrentMonthStatement(int clientId)` | `Result<MonthlyStatement>` | Proforma/VAT pair for the current month |
| `GetMonthlyStatement(int clientId, int month, int year)` | `Result<MonthlyStatement>` | Proforma/VAT pair for a specific month |
| `GetInvoice(int invoiceId, DocumentKind kind)` | `Result<Invoice>` | Single invoice by ID and type |
| `GetCurrentlyPaidInvoices(int clientId, List<int> unPayedProInvoiceIds)` | `Result<IReadOnlyList<MonthlyStatement>>` | Payment status for given proforma IDs |
| `GetAllStatements(int clientId)` | `Result<IReadOnlyList<MonthlyStatement>>` | All monthly statements for a client |

All methods return `Task<Result<T>>` using [CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions).

---

## Models

### Invoice

A value object representing a single invoice.

| Property      | Type       | Description        |
|---------------|------------|--------------------|
| `Identifier`  | `int`      | Invoice ID         |
| `Number`      | `string`   | Invoice number     |
| `IssueDate`   | `DateTime` | Issue date         |
| `PaymentDate` | `DateTime` | Payment due date   |
| `PriceNet`    | `decimal`  | Net price          |

### MonthlyStatement

Aggregates a proforma invoice with its matching VAT invoice.

| Property          | Type       | Description                                    |
|-------------------|------------|------------------------------------------------|
| `ProFormaInvoice` | `Invoice`  | Proforma invoice                               |
| `Invoice`         | `Invoice?` | VAT invoice (`null` if unpaid)                 |
| `IsPaid`          | `bool`     | Whether the proforma has a matching VAT invoice |

### DocumentKind

Supported invoice document types:

| Value            | API Value          | Description          |
|------------------|--------------------|----------------------|
| `Vat`            | `vat`              | VAT invoice          |
| `Proforma`       | `proforma`         | Proforma invoice     |
| `Bill`           | `bill`             | Bill                 |
| `Receipt`        | `receipt`          | Receipt              |
| `Advance`        | `advance`          | Advance invoice      |
| `Correction`     | `correction`       | Correction invoice   |
| `VatMp`          | `vat_mp`           | VAT MP invoice       |
| `InvoiceOther`   | `invoice_other`    | Other invoice        |
| `VatMargin`      | `vat_margin`       | VAT margin invoice   |
| `Kp`             | `kp`               | Cash receipt (KP)    |
| `Kw`             | `kw`               | Cash disbursement (KW) |
| `Final`          | `final`            | Final invoice        |
| `Estimate`       | `estimate`         | Estimate             |
| `AccountingNote` | `accounting_note`  | Accounting note      |

### Period

Time period filters used internally for API queries:

| Value          | API Value        |
|----------------|------------------|
| `ThisMonth`    | `this_month`     |
| `Last30Days`   | `last_30_days`   |
| `LastMonth`    | `last_month`     |
| `ThisYear`     | `this_year`      |
| `LastYear`     | `last_year`      |
| `All`          | `all`            |

---

## Error Handling

### Result Pattern

All client methods return `Task<Result<T>>` instead of throwing exceptions. This enables functional error handling:

```csharp
var result = await client.GetInvoice(invoiceId: 123, kind: DocumentKind.Vat);

if (result.IsSuccess)
{
    var invoice = result.Value;
    // Process invoice
}
else
{
    var error = result.Error;
    // Handle error
}
```

### Retry Policy

The library includes a built-in Polly retry policy:

- **Retries:** 5 attempts
- **Backoff:** Exponential — `2^attempt` seconds (2s, 4s, 8s, 16s, 32s)
- **Handles:** Transient HTTP errors and `429 TooManyRequests`

No additional configuration is needed — the retry policy is registered automatically via `AddFakturownia<TSettings>()`.

---

## Health Checks

The library registers a `FakturowniaHealthCheck` that validates connectivity to the Fakturownia API. It integrates with the standard ASP.NET Core health check infrastructure:

```csharp
app.MapHealthChecks("/health");
```

---

## Architecture

```
SOFTURE.Fakturownia/
├── Abstractions/
│   ├── IFakturowniaClient.cs    # Public client interface
│   └── IFakturowniaApi.cs       # Refit HTTP API definition
├── Delegates/
│   └── GetApiTokenHandler.cs    # Injects API token into requests
├── HealthChecks/
│   └── FakturowniaHealthCheck.cs
├── Mappers/
│   └── InvoiceDetailsMapper.cs  # API DTO → domain model mapping
├── Models/
│   ├── Api/
│   │   ├── Enums/               # DocumentKind, Period
│   │   └── InvoiceDetails.cs    # API response DTO
│   └── Client/
│       ├── Invoice.cs           # Domain value object
│       └── MonthlyStatement.cs  # Proforma + VAT pair
├── Settings/
│   ├── FakturowniaSettings.cs   # Configuration POCO
│   └── IFakturowniaSettings.cs  # Settings contract
├── DependencyInjection.cs       # Service registration
└── FakturowniaClient.cs         # Client implementation
```

**Key design decisions:**

- **[Refit](https://github.com/reactiveui/refit)** — declarative HTTP client generation from the `IFakturowniaApi` interface
- **[Polly](https://github.com/App-vNext/Polly)** — resilience and retry policies integrated via `Microsoft.Extensions.Http.Polly`
- **Mapper pattern** — separates the API response model (`InvoiceDetails`, 100+ fields) from the clean domain model (`Invoice`, 5 fields)
- **Value Objects** — `Invoice` extends `ValueObject` for structural equality
- **DelegatingHandler** — `GetApiTokenHandler` injects the `api_token` query parameter transparently

---

## Contributing

Contributions are welcome! Here's how to get started:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/my-feature`)
3. **Commit** your changes (`git commit -m 'Add my feature'`)
4. **Push** to the branch (`git push origin feature/my-feature`)
5. **Open** a Pull Request

### Building Locally

```bash
cd FAKTUROWNIA
dotnet restore
dotnet build
```

### Running the Playground

```bash
cd FAKTUROWNIA/SOFTURE.Fakturownia.Playground
# Update appsettings.json with your API credentials
dotnet run
```

---

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- [Fakturownia](https://fakturownia.pl) — the invoicing platform this library integrates with
- [Refit](https://github.com/reactiveui/refit) — type-safe REST client
- [Polly](https://github.com/App-vNext/Polly) — resilience and transient-fault-handling
- [CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions) — functional programming primitives for C#
