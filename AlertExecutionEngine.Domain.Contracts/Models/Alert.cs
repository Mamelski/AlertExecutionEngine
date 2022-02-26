namespace AlertExecutionEngine.Domain.Contracts.Models;

// I could discuss if AlertState, SecondsSinceLastExecution, SecondsSinceLastNotification should have setters and getters
// or should there be methods hiding operations on those properties.
// This problem is stated here: https://www.yegor256.com/2014/09/16/getters-and-setters-are-evil.html (and in many other places).
// For me using getters and setters in this case was convenient, I do not think it matters in small project like this. I know it is not "clear"
// and we should "Tell, Don't ask" but sometimes it is better to be pragmatic than dogmatic (or maybe it is just mine excuse for bad design :)).
public record Alert(
    string Name,
    string Metric,
    int IntervalInSeconds,
    int RepeatIntervalInSeconds,
    Threshold WarningThreshold,
    Threshold CriticalThreshold)
{
    private readonly object _secondsSinceLastExecutionLock = new();
    private int _secondsSinceLastExecution;
    public AlertState AlertState { get; set; } = AlertState.Pass;

    public int SecondsSinceLastExecution
    {
        get
        {
            lock(_secondsSinceLastExecutionLock)
            {
                return _secondsSinceLastExecution;
            }
        }
        set
        {
            lock(_secondsSinceLastExecutionLock)
            {
                _secondsSinceLastExecution = value;
            }
        }
    }
}