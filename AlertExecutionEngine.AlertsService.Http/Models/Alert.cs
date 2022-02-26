namespace AlertExecutionEngine.AlertsService.Http.Models;

public record Alert(
    string name,
    string query,
    int intervalSecs,
    int repeatIntervalSecs,
    Threshold warn,
    Threshold critical);