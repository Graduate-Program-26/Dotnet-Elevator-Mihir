public interface IConsoleRenderer
{
    void RenderStatus(IEnumerable<ElevatorStatus> statuses);
    void RenderMessage(string message);
    void RenderError(string message);
}