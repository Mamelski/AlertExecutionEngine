using Microsoft.Extensions.Configuration;

namespace AlertExecutionEngine.Core.Configuration;

public record MonitoringEngineConfiguration
{
    public MonitoringEngineConfiguration(IConfiguration configuration)
    {
        // MonitoringEngineConfiguration binds itself, I prefer this pattern instead IOptions pattern forced by Microsoft
        configuration.GetSection(nameof(MonitoringEngineConfiguration)).Bind(this);
    }

    public int MaximumNumberOfConcurrentTasks { get; set; } = 2;
}