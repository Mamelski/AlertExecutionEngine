namespace AlertExecutionEngine.Domain.Contracts.Models;

// I intentionally did not start from 0, so if in the future there will be some database(or some other Input/Output) with this enum as int
// and it will have default value (which is 0) it won't be anything significant and parsing will throw exception. Maybe this is too defensive, maybe not.
// Usually I also have Unknown = 0 everywhere, but it complicates logic a little bit but it is very helpful in big projects.
public enum AlertState
{
    Pass = 1,
    Warning = 2,
    Critical = 3
}