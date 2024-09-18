using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SOFTURE.Fakturownia;
using SOFTURE.Fakturownia.Playground;
using SOFTURE.Settings.Extensions;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build()
    .ValidateSettings<PlaygroundSettings>();

var serviceCollection = new ServiceCollection();

ConfigureServices(serviceCollection, configuration);

var serviceProvider = serviceCollection.BuildServiceProvider();
var playground = serviceProvider.GetService<Playground>();

if (playground == null) return;

await playground.Run();

return;

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services
        .ConfigureSettings<PlaygroundSettings>(configuration)
        .AddFakturownia<PlaygroundSettings>()
        .AddScoped<Playground>();
}