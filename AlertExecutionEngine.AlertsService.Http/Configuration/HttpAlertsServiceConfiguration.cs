using Microsoft.Extensions.Configuration;

namespace AlertExecutionEngine.AlertsService.Http.Configuration;

public record HttpAlertsServiceConfiguration
{
    // In general this should point to a place where configuration can be acquired from (instead of just saying it is not here)
    private const string NotInitialized = "Not Initialized!";

    public HttpAlertsServiceConfiguration(IConfiguration configuration)
    {
        // HttpAlertsServiceConfiguration binds itself, I prefer this pattern instead IOptions pattern forced by Microsoft
        configuration.GetSection(nameof(HttpAlertsServiceConfiguration)).Bind(this);
    }

    public string BaseUrl { get; set; } = NotInitialized;
    public string GetAllAlertsEndpoint { get; set; } = NotInitialized;
    public string GetMetricEndpoint { get; set; } = NotInitialized;
    public string NotifyEndpoint { get; set; } = NotInitialized;
    public string ResolveEndpoint { get; set; } = NotInitialized;
}