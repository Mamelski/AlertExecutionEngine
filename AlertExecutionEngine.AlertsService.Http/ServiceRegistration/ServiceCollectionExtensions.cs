using AlertExecutionEngine.AlertsService.Http.Configuration;
using AlertExecutionEngine.Domain.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlertExecutionEngine.AlertsService.Http.ServiceRegistration;

public static class ServiceCollectionExtensions
{
    // Registers all services from AlertExecutionEngine.AlertsService.Http
    // Other projects (AlertExecutionEngine.Cli in our case, but possibly some RestApi, gRPC, test project, etc.) that want to use services from
    // this library can call this method instead of registering each service individually. It means that this library can basically register
    // itself in DI container and hides all registration details (although it still requires the correct configuration to be passed).
    public static IServiceCollection AddHttpAlertsService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var httpAlertsServiceConfiguration = new HttpAlertsServiceConfiguration(configuration);
        services.AddSingleton(httpAlertsServiceConfiguration);

        services.AddHttpClient();
        services.AddTransient<IExternalAlertsService, HttpAlertsService>();

        return services;
    }
}