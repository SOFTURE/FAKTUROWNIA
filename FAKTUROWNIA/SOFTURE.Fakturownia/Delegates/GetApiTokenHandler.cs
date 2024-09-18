using System.Web;
using Microsoft.Extensions.Options;
using SOFTURE.Fakturownia.Settings;

namespace SOFTURE.Fakturownia.Delegates;

internal sealed class GetApiTokenHandler(IOptions<FakturowniaSettings> options) : DelegatingHandler
{
    private readonly FakturowniaSettings _fakturowniaSettings = options.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method != HttpMethod.Get)
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        
        var uriBuilder = new UriBuilder(request.RequestUri!);

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        
        query["api_token"] = _fakturowniaSettings.ApiKey;
        
        uriBuilder.Query = query.ToString();

        request.RequestUri = uriBuilder.Uri;

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}