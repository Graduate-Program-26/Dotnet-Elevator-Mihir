public class ConsoleInputHandler(
    IElevatorController controller,
    IConsoleRenderer renderer)
{
    private readonly IElevatorController _controller = controller;
    private readonly IConsoleRenderer _renderer = renderer;

    public void HandleCallElevator(int maxFloor)
    {
        Console.WriteLine();

        var floor = PromptInt($"  Enter floor number (1-{maxFloor}): ");
        if (floor is null) return;

        var passengerCount = PromptInt("  Enter number of passengers: ");
        if (passengerCount is null) return;

        var passengers = new List<Passenger>();

        for (int i = 1; i <= passengerCount; i++)
        {
            var destination = PromptInt(
                $"  Enter destination floor for passenger {i} (1-{maxFloor}): ");

            if (destination is null) return;

            passengers.Add(new Passenger(floor.Value, destination.Value));
        }

        try
        {
            _controller.RequestElevator(floor.Value, passengers);
            _renderer.RenderMessage(
                $"Elevator dispatched to floor {floor} — {passengers.Count} passenger(s) boarding.");
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
