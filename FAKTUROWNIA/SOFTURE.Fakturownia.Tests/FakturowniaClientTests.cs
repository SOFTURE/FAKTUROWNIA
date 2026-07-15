using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Models.Api;
using SOFTURE.Fakturownia.Models.Api.Enums;
using Xunit;

namespace SOFTURE.Fakturownia.Tests;

public class FakturowniaClientTests
{
    private const int ClientId = 220215888;

    private readonly IFakturowniaApi _api = Substitute.For<IFakturowniaApi>();
    private readonly FakturowniaClient _client;

    public FakturowniaClientTests()
    {
        _client = new FakturowniaClient(_api, NullLogger<FakturowniaClient>.Instance);
    }

    private void ApiReturns(params InvoiceDetails[] invoices)
    {
        _api.GetInvoicesAsync(Arg.Any<Period>(), Arg.Any<int>(), Arg.Any<DocumentKind[]>())
            .Returns(invoices.AsApiResponse());
    }

    [Fact]
    public async Task GetCurrentMonthStatement_WhenClientHasTwoProFormaInvoices_ReturnsFailureInsteadOfThrowing()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501),
            InvoiceDetailsBuilder.ProForma(id: 502)
        );

        var result = await _client.GetCurrentMonthStatement(ClientId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Multiple proforma invoices (2)");
        result.Error.Should().Contain("501, 502");
        result.Error.Should().Contain(ClientId.ToString());
    }

    [Fact]
    public async Task GetCurrentMonthStatement_WhenProFormaHasTwoVatInvoices_ReturnsFailureInsteadOfThrowing()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501),
            InvoiceDetailsBuilder.Vat(id: 601, fromProFormaId: 501),
            InvoiceDetailsBuilder.Vat(id: 602, fromProFormaId: 501)
        );

        var result = await _client.GetCurrentMonthStatement(ClientId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Multiple vat invoices (2)");
        result.Error.Should().Contain("601, 602");
        result.Error.Should().Contain("proforma invoice with id: 501");
    }

    [Fact]
    public async Task GetCurrentMonthStatement_WhenProFormaHasSingleVatInvoice_ReturnsPaidStatement()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501),
            InvoiceDetailsBuilder.Vat(id: 601, fromProFormaId: 501)
        );

        var result = await _client.GetCurrentMonthStatement(ClientId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.IsPaid.Should().BeTrue();
        result.Value.ProFormaInvoice.Identifier.Should().Be(501);
        result.Value.Invoice!.Identifier.Should().Be(601);
    }

    [Fact]
    public async Task GetCurrentMonthStatement_WhenProFormaHasNoVatInvoice_ReturnsUnpaidStatement()
    {
        ApiReturns(InvoiceDetailsBuilder.ProForma(id: 501));

        var result = await _client.GetCurrentMonthStatement(ClientId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.IsPaid.Should().BeFalse();
        result.Value.ProFormaInvoice.Identifier.Should().Be(501);
    }

    [Fact]
    public async Task GetCurrentMonthStatement_WhenVatInvoiceBelongsToAnotherProForma_ReturnsUnpaidStatement()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501),
            InvoiceDetailsBuilder.Vat(id: 601, fromProFormaId: 999)
        );

        var result = await _client.GetCurrentMonthStatement(ClientId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.IsPaid.Should().BeFalse();
    }

    [Fact]
    public async Task GetCurrentMonthStatement_WhenNoProFormaInvoice_ReturnsSuccessWithNull()
    {
        ApiReturns(InvoiceDetailsBuilder.Vat(id: 601, fromProFormaId: 501));

        var result = await _client.GetCurrentMonthStatement(ClientId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentMonthStatement_WhenNoInvoices_ReturnsSuccessWithNull()
    {
        ApiReturns();

        var result = await _client.GetCurrentMonthStatement(ClientId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentlyPaidInvoices_WhenProFormaHasTwoVatInvoices_ReturnsFailureInsteadOfThrowing()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 265034461),
            InvoiceDetailsBuilder.Vat(id: 701, fromProFormaId: 265034461),
            InvoiceDetailsBuilder.Vat(id: 702, fromProFormaId: 265034461)
        );

        var result = await _client.GetCurrentlyPaidInvoices(ClientId, [265034461]);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Multiple vat invoices (2)");
        result.Error.Should().Contain("701, 702");
        result.Error.Should().Contain("proforma invoice with id: 265034461");
    }

    [Fact]
    public async Task GetCurrentlyPaidInvoices_WhenOneOfManyProFormasIsAmbiguous_ReturnsFailureForWholeBatch()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501),
            InvoiceDetailsBuilder.Vat(id: 601, fromProFormaId: 501),
            InvoiceDetailsBuilder.ProForma(id: 502),
            InvoiceDetailsBuilder.Vat(id: 602, fromProFormaId: 502),
            InvoiceDetailsBuilder.Vat(id: 603, fromProFormaId: 502)
        );

        var result = await _client.GetCurrentlyPaidInvoices(ClientId, [501, 502]);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("proforma invoice with id: 502");
    }

    [Fact]
    public async Task GetCurrentlyPaidInvoices_WhenProFormasArePaid_ReturnsStatementForEach()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501),
            InvoiceDetailsBuilder.Vat(id: 601, fromProFormaId: 501),
            InvoiceDetailsBuilder.ProForma(id: 502),
            InvoiceDetailsBuilder.Vat(id: 602, fromProFormaId: 502)
        );

        var result = await _client.GetCurrentlyPaidInvoices(ClientId, [501, 502]);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().OnlyContain(s => s.IsPaid);
    }

    [Fact]
    public async Task GetCurrentlyPaidInvoices_WhenClientHasNoVatInvoicesAtAll_ReturnsEmptyListInsteadOfFailure()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501),
            InvoiceDetailsBuilder.ProForma(id: 502)
        );

        var result = await _client.GetCurrentlyPaidInvoices(ClientId, [501, 502]);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCurrentlyPaidInvoices_WhenProFormaHasNoVatInvoice_SkipsIt()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501),
            InvoiceDetailsBuilder.Vat(id: 601, fromProFormaId: 501),
            InvoiceDetailsBuilder.ProForma(id: 502)
        );

        var result = await _client.GetCurrentlyPaidInvoices(ClientId, [501, 502]);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value[0].ProFormaInvoice.Identifier.Should().Be(501);
    }

    [Fact]
    public async Task GetMonthlyStatement_WhenMonthHasTwoProFormaInvoices_ReturnsFailureInsteadOfThrowing()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501, issueDate: new DateTime(2026, 6, 5)),
            InvoiceDetailsBuilder.ProForma(id: 502, issueDate: new DateTime(2026, 6, 20))
        );

        var result = await _client.GetMonthlyStatement(ClientId, month: 6, year: 2026);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Multiple proforma invoices (2)");
        result.Error.Should().Contain("06/2026");
    }

    [Fact]
    public async Task GetMonthlyStatement_WhenProFormaFromAnotherMonthExists_IgnoresIt()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501, issueDate: new DateTime(2026, 6, 5)),
            InvoiceDetailsBuilder.ProForma(id: 502, issueDate: new DateTime(2026, 5, 5))
        );

        var result = await _client.GetMonthlyStatement(ClientId, month: 6, year: 2026);

        result.IsSuccess.Should().BeTrue();
        result.Value.ProFormaInvoice.Identifier.Should().Be(501);
    }

    [Fact]
    public async Task GetAllStatements_WhenProFormaHasTwoVatInvoices_ReturnsUnpaidStatementWithoutThrowing()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 501),
            InvoiceDetailsBuilder.Vat(id: 601, fromProFormaId: 501),
            InvoiceDetailsBuilder.Vat(id: 602, fromProFormaId: 501)
        );

        var result = await _client.GetAllStatements(ClientId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value[0].IsPaid.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllStatements_WhenProFormasArePaid_ReturnsStatementsOrderedByIssueDate()
    {
        ApiReturns(
            InvoiceDetailsBuilder.ProForma(id: 502, issueDate: new DateTime(2026, 6, 1)),
            InvoiceDetailsBuilder.Vat(id: 602, fromProFormaId: 502, issueDate: new DateTime(2026, 6, 2)),
            InvoiceDetailsBuilder.ProForma(id: 501, issueDate: new DateTime(2026, 5, 1)),
            InvoiceDetailsBuilder.Vat(id: 601, fromProFormaId: 501, issueDate: new DateTime(2026, 5, 2))
        );

        var result = await _client.GetAllStatements(ClientId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].ProFormaInvoice.Identifier.Should().Be(501);
        result.Value[1].ProFormaInvoice.Identifier.Should().Be(502);
    }
}
