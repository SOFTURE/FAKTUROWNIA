using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Settings;
using SOFTURE.Settings.Extensions;

namespace SOFTURE.Fakturownia.Tests.Pipeline;

internal sealed class FakturowniaPipeline : IDisposable
{
    public const string BaseUrl = "https://softure.fakturownia.invalid";
    public const string ApiKey = "test-api-key";

    private readonly ServiceProvider _serviceProvider;

    public FakturowniaPipeline()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Fakturownia:Url"] = BaseUrl,
                ["Fakturownia:ApiKey"] = ApiKey
            })
            .Build();

        var services = new ServiceCollection();

        services.AddLogging(builder => builder.ClearProviders());
        services.ConfigureSettings<PipelineSettings>(configuration);
        services.AddFakturownia<PipelineSettings>();
        services.ConfigureAll<HttpClientFactoryOptions>(options => options.HttpMessageHandlerBuilderActions.Add(builder => builder.PrimaryHandler = Handler));

        _serviceProvider = services.BuildServiceProvider();
    }

    public StubHttpMessageHandler Handler { get; } = new();

    public IFakturowniaApi Api => _serviceProvider.GetRequiredService<IFakturowniaApi>();

    public IFakturowniaClient Client => _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IFakturowniaClient>();

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }

    internal sealed class PipelineSettings : IFakturowniaSettings
    {
        public FakturowniaSettings Fakturownia { get; init; } = null!;
    }
}
