namespace ElevatorSim.Infrastructure.Logging;
/// <summary>
/// Reads recent log entries from the simulation's log file for display.
/// </summary>
public interface ILogViewer
{
    /// <summary>
    /// Returns the most recent log lines, up to the specified count.
    /// </summary>
    IReadOnlyList<string> GetRecentEntries(int count);
}
