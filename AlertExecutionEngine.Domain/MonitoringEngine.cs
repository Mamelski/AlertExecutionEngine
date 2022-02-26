using System.Timers;
using AlertExecutionEngine.Core.Configuration;
using AlertExecutionEngine.Domain.Contracts;
using AlertExecutionEngine.Domain.Contracts.Models;
using Timer = System.Timers.Timer;

namespace AlertExecutionEngine.Core;

// Main service, monitors alerts, notifies and resolves
public class MonitoringEngine : IMonitoringEngine
{
    private readonly MonitoringEngineConfiguration _configuration;
    private readonly IExternalAlertsService _externalAlertsService;

    private IList<Alert> _alerts = null!;
    private ParallelOptions _parallelOptions = null!;
    private Timer _timer = null!;

    public MonitoringEngine(
        IExternalAlertsService externalAlertsService,
        MonitoringEngineConfiguration configuration)
    {
        _externalAlertsService = externalAlertsService;
        _configuration = configuration;
    }

    // Starts monitoring engine, can be stopped by requesting cancellation on token
    public async Task Start(CancellationToken cancellationToken)
    {
        Console.WriteLine("Engine started");
        _alerts = await _externalAlertsService.GetAllAlerts();

        // Those options will be used by dotnet parallel task library
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _configuration.MaximumNumberOfConcurrentTasks,
            CancellationToken = cancellationToken
        };

        _timer = new Timer();
        _timer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
        _timer.Elapsed += OneSecondElapsed;
        _timer.Start();

        // Do not stop until cancellation requested
        while(!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        _timer.Stop();
        _timer.Dispose();
    }

    private void OneSecondElapsed(object? source, ElapsedEventArgs e)
    {
        Console.WriteLine(".");

        Parallel.ForEachAsync(
            _alerts,
            _parallelOptions,
            async (alert, _) => { await ExecuteAlertIfNeeded(alert); });
    }

    private async Task ExecuteAlertIfNeeded(Alert alert)
    {
        // We know that this is called every 1 second, so we can do +1 to those properties
        // All those properties are behind locks and they are thread safe
        alert.SecondsSinceLastExecution += 1;
        alert.WarningThreshold.SecondsSinceLastNotification += 1;
        alert.CriticalThreshold.SecondsSinceLastNotification += 1;

        if(alert.SecondsSinceLastExecution != alert.IntervalInSeconds)
        {
            // Do nothing, interval did not elapse
            return;
        }

        // This can be here or at the end of this method.
        // It depends if we want count interval from start of the alert execution or from the end of it.
        // If execution takes more time that interval, then this line here will cause problems (more and more executions will be in progress)
        alert.SecondsSinceLastExecution = 0;
        Console.WriteLine($"Executing {alert.Name}");

        var metric = await _externalAlertsService.GetMetric(alert.Metric);
        var newAlertState = DetermineAlertState(alert, metric);

        await NotifyAlertIfNeeded(alert, newAlertState);
        await ResolveAlertIfNeeded(alert, newAlertState);

        alert.AlertState = newAlertState;
    }

    private static AlertState DetermineAlertState(Alert alert, Metric metric)
    {
        if(metric.Value <= alert.WarningThreshold.Value)
        {
            return AlertState.Pass;
        }

        return metric.Value <= alert.CriticalThreshold.Value
            ? AlertState.Warning
            : AlertState.Critical;
    }

    private async Task NotifyAlertIfNeeded(Alert alert, AlertState newAlertState)
    {
        var oldAlertState = alert.AlertState;

        // transition to warning
        if(oldAlertState is AlertState.Pass && newAlertState is AlertState.Warning)
        {
            await _externalAlertsService.SendNotification(new Notification(alert.Name, alert.WarningThreshold.Message));
            alert.WarningThreshold.SecondsSinceLastNotification = 0;
        }

        // transition to critical
        if(oldAlertState is AlertState.Pass or AlertState.Warning && newAlertState is AlertState.Critical)
        {
            await _externalAlertsService.SendNotification(
                new Notification(alert.Name, alert.CriticalThreshold.Message));
            alert.CriticalThreshold.SecondsSinceLastNotification = 0;
        }

        // continuous warning 
        if(oldAlertState is AlertState.Warning && newAlertState is AlertState.Warning)
        {
            if(alert.WarningThreshold.SecondsSinceLastNotification >= alert.RepeatIntervalInSeconds)
            {
                await _externalAlertsService.SendNotification(
                    new Notification(alert.Name, alert.WarningThreshold.Message));
                alert.WarningThreshold.SecondsSinceLastNotification = 0;
            }
        }

        // continuous critical 
        if(oldAlertState is AlertState.Critical && newAlertState is AlertState.Critical)
        {
            if(alert.CriticalThreshold.SecondsSinceLastNotification >= alert.RepeatIntervalInSeconds)
            {
                await _externalAlertsService.SendNotification(
                    new Notification(alert.Name, alert.CriticalThreshold.Message));
                alert.CriticalThreshold.SecondsSinceLastNotification = 0;
            }
        }
    }

    private async Task ResolveAlertIfNeeded(Alert alert, AlertState newAlertState)
    {
        var oldAlertState = alert.AlertState;
        if(oldAlertState is AlertState.Warning or AlertState.Critical && newAlertState is AlertState.Pass)
        {
            await _externalAlertsService.ResolveAlert(alert.Name);
        }
    }
}