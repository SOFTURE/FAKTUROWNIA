using System.Net;
using System.Web;
using FluentAssertions;
using SOFTURE.Fakturownia.Models.Api.Enums;
using Xunit;

namespace SOFTURE.Fakturownia.Tests.Pipeline;

public sealed class FakturowniaPipelineTests : IDisposable
{
    private const int ClientId = 220215888;

    private const string InvoicesJson =
        """
        [
          {
            "id": 501,
            "number": "PRO 1/06/2026",
            "kind": "proforma",
            "price_net": "708.00",
            "price_gross": "870.84",
            "issue_date": "2026-06-01",
            "payment_to": "2026-06-15",
            "created_at": "2026-06-01T10:15:30.000+02:00",
            "client_id": 220215888,
            "from_invoice_id": null,
            "overdue?": false,
            "calculating_strategy": { "position": "default", "sum": "sum", "invoice_form_price_kind": "net" },
            "gtu_codes": [],
            "app": null,
            "unknown_field_added_by_fakturownia": { "nested": [1, 2, 3] }
          },
          {
            "id": 601,
            "number": "FV 1/06/2026",
            "kind": "vat",
            "price_net": "708.00",
            "issue_date": "2026-06-10",
            "payment_to": "2026-06-24",
            "created_at": "2026-06-10T08:00:00.000+02:00",
            "client_id": 220215888,
            "from_invoice_id": 501
          }
        ]
        """;

    private const string SingleInvoiceJson =
        """
        {
          "id": 601,
          "number": "FV 1/06/2026",
          "kind": "vat",
          "price_net": "1234.56",
          "issue_date": "2026-06-10",
          "payment_to": "2026-06-24",
          "created_at": "2026-06-10T08:00:00.000+02:00",
          "client_id": 220215888,
          "from_invoice_id": 501
        }
        """;

    private readonly FakturowniaPipeline _pipeline = new();

    public void Dispose()
    {
        _pipeline.Dispose();
    }

    [Fact]
    public async Task GetInvoicesAsync_SendsGetToInvoicesEndpointOfConfiguredHost()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, "[]");

        await _pipeline.Api.GetInvoicesAsync(Period.ThisMonth, ClientId, [DocumentKind.Proforma, DocumentKind.Vat]);

        var request = _pipeline.Handler.Requests.Should().ContainSingle().Subject;

        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri!.GetLeftPart(UriPartial.Path).Should().Be($"{FakturowniaPipeline.BaseUrl}/invoices.json");
    }

    [Fact]
    public async Task GetInvoicesAsync_SendsEnumMemberValuesAndAliasedNamesInQuery()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, "[]");

        await _pipeline.Api.GetInvoicesAsync(Period.ThisMonth, ClientId, [DocumentKind.Proforma, DocumentKind.Vat]);

        var query = QueryOf(_pipeline.Handler.Requests.Single());

        query.AllKeys.Should().BeEquivalentTo("period", "client_id", "kinds[]", "api_token");
        query["period"].Should().Be("this_month");
        query["client_id"].Should().Be("220215888");
        query.GetValues("kinds[]").Should().Equal("proforma", "vat");
    }

    [Theory]
    [InlineData(Period.All, "all")]
    [InlineData(Period.Last30Days, "last_30_days")]
    [InlineData(Period.LastMonth, "last_month")]
    [InlineData(Period.ThisYear, "this_year")]
    [InlineData(Period.LastYear, "last_year")]
    public async Task GetInvoicesAsync_SendsEnumMemberValueForEveryPeriod(Period period, string expected)
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, "[]");

        await _pipeline.Api.GetInvoicesAsync(period, ClientId, [DocumentKind.Vat]);

        QueryOf(_pipeline.Handler.Requests.Single())["period"].Should().Be(expected);
    }

    [Theory]
    [InlineData(DocumentKind.VatMp, "vat_mp")]
    [InlineData(DocumentKind.InvoiceOther, "invoice_other")]
    [InlineData(DocumentKind.AccountingNote, "accounting_note")]
    public async Task GetInvoiceAsync_SendsEnumMemberValueForMultiWordDocumentKind(DocumentKind kind, string expected)
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, SingleInvoiceJson);

        await _pipeline.Api.GetInvoiceAsync(601, [kind]);

        QueryOf(_pipeline.Handler.Requests.Single()).GetValues("kinds[]").Should().Equal(expected);
    }

    [Fact]
    public async Task GetInvoiceAsync_SendsInvoiceIdInPathAndApiTokenInQuery()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, SingleInvoiceJson);

        await _pipeline.Api.GetInvoiceAsync(601, [DocumentKind.Vat]);

        var request = _pipeline.Handler.Requests.Single();
        var query = QueryOf(request);

        request.RequestUri!.GetLeftPart(UriPartial.Path).Should().Be($"{FakturowniaPipeline.BaseUrl}/invoices/601.json");
        query["api_token"].Should().Be(FakturowniaPipeline.ApiKey);
        query.GetValues("kinds[]").Should().Equal("vat");
    }

    [Fact]
    public async Task GetInvoicesAsync_DeserializesSnakeCaseFieldsAndStringEnumKinds()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, InvoicesJson);

        var response = await _pipeline.Api.GetInvoicesAsync(Period.All, ClientId, [DocumentKind.Proforma, DocumentKind.Vat]);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.Error.Should().BeNull();

        var invoices = response.Content!.ToList();

        invoices.Should().HaveCount(2);

        var proForma = invoices[0];

        proForma.Id.Should().Be(501);
        proForma.Number.Should().Be("PRO 1/06/2026");
        proForma.Kind.Should().Be(DocumentKind.Proforma);
        proForma.PriceNet.Should().Be("708.00");
        proForma.PriceGross.Should().Be("870.84");
        proForma.IssueDate.Should().Be(new DateTime(2026, 6, 1));
        proForma.PaymentTo.Should().Be(new DateTime(2026, 6, 15));
        proForma.ClientId.Should().Be(ClientId);
        proForma.FromInvoiceId.Should().BeNull();
        proForma.Overdue.Should().BeFalse();
        proForma.CalculatingStrategy!.InvoiceFormPriceKind.Should().Be("net");
        proForma.GtuCodes.Should().BeEmpty();

        var vat = invoices[1];

        vat.Id.Should().Be(601);
        vat.Kind.Should().Be(DocumentKind.Vat);
        vat.FromInvoiceId.Should().Be(501);
        vat.IssueDate.Should().Be(new DateTime(2026, 6, 10));
    }

    [Fact]
    public async Task GetInvoicesAsync_WhenFakturowniaReturnsError_ExposesStatusAndBodyWithoutThrowing()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.Unauthorized, """{"code":"error","message":"invalid api_token"}""");

        var response = await _pipeline.Api.GetInvoicesAsync(Period.All, ClientId, [DocumentKind.Vat]);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Content.Should().BeNull();
        response.HasResponseError(out var error).Should().BeTrue();
        error!.Content.Should().Be("""{"code":"error","message":"invalid api_token"}""");
    }

    [Fact]
    public async Task GetInvoice_WhenFakturowniaIsTemporarilyUnavailable_RetriesAndReturnsInvoice()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.ServiceUnavailable, "");
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, SingleInvoiceJson);

        var result = await _pipeline.Client.GetInvoice(601, DocumentKind.Vat);

        result.IsSuccess.Should().BeTrue();
        result.Value.Identifier.Should().Be(601);
        _pipeline.Handler.Requests.Should().HaveCount(2);
        _pipeline.Handler.Requests.Should().AllSatisfy(request => QueryOf(request)["api_token"].Should().Be(FakturowniaPipeline.ApiKey));
    }

    [Fact]
    public async Task GetInvoice_WhenFakturowniaRateLimits_RetriesAndReturnsInvoice()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.TooManyRequests, "");
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, SingleInvoiceJson);

        var result = await _pipeline.Client.GetInvoice(601, DocumentKind.Vat);

        result.IsSuccess.Should().BeTrue();
        _pipeline.Handler.Requests.Should().HaveCount(2);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task GetInvoice_WhenFakturowniaRejectsRequest_DoesNotRetry(HttpStatusCode statusCode)
    {
        _pipeline.Handler.RespondWith(statusCode, """{"code":"error"}""");

        var result = await _pipeline.Client.GetInvoice(601, DocumentKind.Vat);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("""Failed to get invoice with id: 601 - {"code":"error"}""");
        _pipeline.Handler.Requests.Should().ContainSingle();
    }

    [Fact]
    public async Task GetInvoice_WhenServerErrorsTwice_RetriesUntilSuccess()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.InternalServerError, "");
        _pipeline.Handler.RespondWith(HttpStatusCode.BadGateway, "");
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, SingleInvoiceJson);

        var result = await _pipeline.Client.GetInvoice(601, DocumentKind.Vat);

        result.IsSuccess.Should().BeTrue();
        _pipeline.Handler.Requests.Should().HaveCount(3);
    }

#if NET10_0_OR_GREATER
    [Fact]
    public async Task GetInvoice_WhenFakturowniaReturnsDuplicateJsonKeys_ReturnsMissingInvoiceFailureInsteadOfThrowing()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, """{"id": 601, "id": 602, "number": "FV 1/06/2026", "kind": "vat", "price_net": "1.00"}""");

        var result = await _pipeline.Client.GetInvoice(601, DocumentKind.Vat);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Missing Vat invoice with id: 601");
    }
#endif

    [Fact]
    public async Task GetInvoice_WhenRequestFailsBeforeResponse_ReturnsFailureInsteadOfThrowing()
    {
        _pipeline.Handler.FailWith(new TaskCanceledException("The request was canceled due to the configured HttpClient.Timeout"));

        var result = await _pipeline.Client.GetInvoice(601, DocumentKind.Vat);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Failed to get invoice with id: 601 - request failed before response: The request was canceled due to the configured HttpClient.Timeout");
    }

    [Fact]
    public async Task GetInvoice_WhenFakturowniaReturnsMalformedJson_ReturnsMissingInvoiceFailure()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, "{\"id\": \"not-a-number\"");

        var result = await _pipeline.Client.GetInvoice(601, DocumentKind.Vat);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Missing Vat invoice with id: 601");
    }

    [Fact]
    public async Task GetCurrentMonthStatement_ThroughRealPipeline_ReturnsPaidStatementMappedFromJson()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, InvoicesJson);

        var result = await _pipeline.Client.GetCurrentMonthStatement(ClientId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsPaid.Should().BeTrue();
        result.Value.ProFormaInvoice.Identifier.Should().Be(501);
        result.Value.ProFormaInvoice.Number.Should().Be("PRO 1/06/2026");
        result.Value.ProFormaInvoice.IssueDate.Should().Be(new DateTime(2026, 6, 1));
        result.Value.ProFormaInvoice.PaymentDate.Should().Be(new DateTime(2026, 6, 15));
        result.Value.Invoice!.Identifier.Should().Be(601);
        result.Value.Invoice.Number.Should().Be("FV 1/06/2026");
        QueryOf(_pipeline.Handler.Requests.Single())["period"].Should().Be("this_month");
    }

    [Fact]
    public async Task GetInvoice_ThroughRealPipeline_ReturnsInvoiceMappedFromJson()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, SingleInvoiceJson);

        var result = await _pipeline.Client.GetInvoice(601, DocumentKind.Vat);

        result.IsSuccess.Should().BeTrue();
        result.Value.Identifier.Should().Be(601);
        result.Value.Number.Should().Be("FV 1/06/2026");
        result.Value.IssueDate.Should().Be(new DateTime(2026, 6, 10));
        result.Value.PaymentDate.Should().Be(new DateTime(2026, 6, 24));
    }

    [Fact]
    public async Task GetInvoice_WhenFakturowniaReturnsNotFound_ReturnsFailureWithResponseBody()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.NotFound, """{"code":"error","message":"not found"}""");

        var result = await _pipeline.Client.GetInvoice(999, DocumentKind.Vat);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("""Failed to get invoice with id: 999 - {"code":"error","message":"not found"}""");
    }

    [Fact]
    public async Task GetInvoice_WhenFakturowniaReturnsNullBody_ReturnsMissingInvoiceFailure()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, "null");

        var result = await _pipeline.Client.GetInvoice(601, DocumentKind.Vat);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Missing Vat invoice with id: 601");
    }

    [Fact]
    public async Task GetInvoice_WhenFakturowniaReturnsEmptyBody_ReturnsMissingInvoiceFailure()
    {
        _pipeline.Handler.RespondWithEmptyBody(HttpStatusCode.OK);

        var result = await _pipeline.Client.GetInvoice(601, DocumentKind.Vat);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Missing Vat invoice with id: 601");
    }

    [Fact]
    public async Task GetCurrentMonthStatement_WhenFakturowniaReturnsEmptyList_ReturnsSuccessWithNull()
    {
        _pipeline.Handler.RespondWith(HttpStatusCode.OK, "[]");

        var result = await _pipeline.Client.GetCurrentMonthStatement(ClientId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    private static System.Collections.Specialized.NameValueCollection QueryOf(HttpRequestMessage request)
    {
        return HttpUtility.ParseQueryString(request.RequestUri!.Query);
    }
}
