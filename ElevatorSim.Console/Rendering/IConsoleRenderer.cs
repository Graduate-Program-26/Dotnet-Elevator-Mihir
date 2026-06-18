using ElevatorSim.Domain.Models;

namespace ElevatorSim.Rendering;

public interface IConsoleRenderer
{
    void RenderStatus(IEnumerable<ElevatorStatus> statuses);
    void RenderMessage(string message);
    void RenderError(string message);
    void RenderLogs(IReadOnlyList<string> logEntries);
}
