namespace AlertExecutionEngine.AlertsService.Http.Models;

// Maps types between AlertsService.Http and Domain.
// HttpAlertsService uses its own types (based on models from http alerts api provided in this interview task),
// but implements IExternalAlertsService interface from domain (with domain models as parameters/return values).
// Without separate models every change in the http alert api models will require changes in domain models
// and this is something we do not want. Domain cannot depend on external service (to some extend of course).
public static class ModelsMapper
{
    public static Domain.Contracts.Models.Alert ToDomainModel(this Alert alert)
    {
        return new Domain.Contracts.Models.Alert(
            alert.name,
            alert.query,
            alert.intervalSecs,
            alert.repeatIntervalSecs,
            alert.warn.ToDomainModel(),
            alert.critical.ToDomainModel());
    }

    private static Domain.Contracts.Models.Threshold ToDomainModel(this Threshold threshold)
    {
        return new Domain.Contracts.Models.Threshold(
            threshold.value,
            threshold.message);
    }

    public static Domain.Contracts.Models.Metric ToDomainModel(this Metric metric)
    {
        return new Domain.Contracts.Models.Metric(metric.value);
    }

    public static Notification ToAlertServiceModel(this Domain.Contracts.Models.Notification notification)
    {
        return new Notification(
            notification.AlertName,
            notification.Message);
    }
}