using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;
using SOFTURE.Common.HealthCheck;
using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Delegates;
using SOFTURE.Fakturownia.HealthChecks;
using SOFTURE.Fakturownia.Settings;
using SOFTURE.Settings.Extensions;

namespace SOFTURE.Fakturownia
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFakturownia<TSettings>(this IServiceCollection services)
            where TSettings : IFakturowniaSettings
        {
            var settings = services.GetSettings<TSettings, FakturowniaSettings>(x => x.Fakturownia);

            services.AddCommonHealthCheck<FakturowniaHealthCheck>();
            
            services.AddTransient<GetApiTokenHandler>();

            services
                .AddRefitGeneratedClient<IFakturowniaApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.Url))
                .AddHttpMessageHandler<GetApiTokenHandler>()
                .AddResilienceHandler("fakturownia", pipeline => pipeline.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 5,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromSeconds(2),
                    UseJitter = false,
                    ShouldRetryAfterHeader = false
                }));
            
            services.AddScoped<IFakturowniaClient, FakturowniaClient>();    
            
            return services;
        }
    }
}