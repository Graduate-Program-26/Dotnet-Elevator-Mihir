public class ConsoleInputHandler(
    IElevatorController controller,
    IConsoleRenderer renderer)
{
    private readonly IElevatorController _controller = controller;
    private readonly IConsoleRenderer _renderer = renderer;

    public void HandleCallElevator(int maxFloor)
    {
        Console.WriteLine();

        var floor = PromptInt(
            $"  Enter floor number (1-{maxFloor}): ");

        if (floor is null) return;

        var passengers = PromptInt(
            "  Enter number of passengers: ");

        if (passengers is null) return;

        try
        {
            _controller.RequestElevator(floor.Value, passengers.Value);
            _renderer.RenderMessage(
                $"Elevator dispatched to floor {floor}.");
        }
        catch (InvalidFloorException ex)
        {
            _renderer.RenderError(ex.Message);
        }
        catch (CapacityExceededException ex)
        {
            _renderer.RenderError(ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _renderer.RenderError(ex.Message);
        }
    }

    private int? PromptInt(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();

        if (int.TryParse(input, out var value))
            return value;

        _renderer.RenderError(
            $"'{input}' is not a valid number. Please try again.");
        return null;
    }
}
