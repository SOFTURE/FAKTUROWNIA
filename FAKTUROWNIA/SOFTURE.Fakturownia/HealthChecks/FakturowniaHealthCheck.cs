using System.Net;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using SOFTURE.Common.HealthCheck.Core;
using SOFTURE.Fakturownia.Settings;

namespace SOFTURE.Fakturownia.HealthChecks;

internal class FakturowniaHealthCheck(HttpClient httpClient, IOptions<FakturowniaSettings> settings) : CheckBase
{
    protected override async Task<Result> Check()
    {
        var fakturowniaSettings = settings.Value;
        
        var uri = new UriBuilder(fakturowniaSettings.Url) { Path = "/" }.Uri;
        
        var response = await httpClient.GetAsync(uri);
        
        return response.StatusCode == HttpStatusCode.OK
            ? Result.Success() 
            : Result.Failure("Fakturownia API is not available");
    }
}