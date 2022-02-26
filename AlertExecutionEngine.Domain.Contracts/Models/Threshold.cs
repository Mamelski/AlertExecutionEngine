namespace AlertExecutionEngine.Domain.Contracts.Models;

public record Threshold(double Value, string Message)
{
    private readonly object _secondsSinceLastNotificationLock = new();
    private int _secondsSinceLastNotification;

    public int SecondsSinceLastNotification
    {
        get
        {
            lock(_secondsSinceLastNotificationLock)
            {
                return _secondsSinceLastNotification;
            }
        }
        set
        {
            lock(_secondsSinceLastNotificationLock)
            {
                _secondsSinceLastNotification = value;
            }
        }
    }
}