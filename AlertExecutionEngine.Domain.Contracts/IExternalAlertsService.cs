using AlertExecutionEngine.Domain.Contracts.Models;

namespace AlertExecutionEngine.Domain.Contracts;

// Interface for external source of alerts and metrics and place to send notifications and resolve alerts.
// This could be 2 or 3 interfaces maybe (ISP!). I can imagine, that one service/component/object is source of alerts
// and second service/component/object is responsible for receiving notifications/resolving them.
// This does not matter so much in scope of this exercise, but I would think about it in real project.
public interface IExternalAlertsService
{
    Task<IList<Alert>> GetAllAlerts();
    Task<Metric> GetMetric(string metricName);
    Task SendNotification(Notification notification);
    Task ResolveAlert(string alertName);
}