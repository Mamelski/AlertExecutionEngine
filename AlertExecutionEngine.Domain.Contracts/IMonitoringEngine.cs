namespace AlertExecutionEngine.Domain.Contracts;

public interface IMonitoringEngine
{
    Task Start(CancellationToken cancellationToken);
}