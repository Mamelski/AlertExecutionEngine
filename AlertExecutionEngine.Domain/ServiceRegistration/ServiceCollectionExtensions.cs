using AlertExecutionEngine.Core.Configuration;
using AlertExecutionEngine.Domain.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlertExecutionEngine.Core.ServiceRegistration;

public static class ServiceCollectionExtensions
{
    // Registers all services from AlertsExecutionEngine.Domain
    // Other projects (AlertExecutionEngine.Cli in our case, but possibly some RestApi, gRPC, test project, etc.) that want to use services from
    // this library can call this method instead of registering each service individually. It means that this library can basically register
    // itself in DI container and hides all registration details (although it still requires the correct configuration to be passed).ne
    public static IServiceCollection AddDomainServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var monitoringEngineConfiguration = new MonitoringEngineConfiguration(configuration);
        services.AddSingleton(monitoringEngineConfiguration);

        services.AddTransient<IMonitoringEngine, MonitoringEngine>();
        return services;
    }
}