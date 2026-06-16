public class ConsoleInputHandler(
    IElevatorController controller,
    IConsoleRenderer renderer)
{
    private readonly IElevatorController _controller = controller;
    private readonly IConsoleRenderer _renderer = renderer;

    public void HandleCallElevator(int maxFloor)
    {
        Console.WriteLine();

        var floor = PromptFloor($"  Enter floor number (1-{maxFloor}): ", 1, maxFloor);
        if (floor is null)
        {
            return;
        }
        if (floor > maxFloor || floor < 1)
        {
            throw new InvalidFloorException(floor.GetValueOrDefault());
        }

        var passengerCount = PromptInt("  Enter number of passengers: ");
        if (passengerCount is null)
        {
            return;
        }

        var passengers = new List<Passenger>();

        for (int i = 1; i <= passengerCount; i++)
        {
            var destination = PromptFloor($"  Enter destination floor (1-{maxFloor}): ", 1, maxFloor);

            if (destination is null)
            {
                return;
            }
            if (floor == destination)
            {
                HandleException($"Destination floor cannot be the same as origin floor {floor}.");
                return;
            }

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
            HandleException(ex.Message);
        }
        catch (CapacityExceededException ex)
        {
            HandleException(ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            HandleException(ex.Message);
        }
    }

    private int? PromptInt(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();

        if (int.TryParse(input, out var value))
        {
            return value;
        }

        _renderer.RenderError($"'{input}' is not a valid number. Please enter a whole number.");
        Console.WriteLine("  Press any key to continue...");
        Console.ReadKey();
        return null;
    }

    private void HandleException(string message)
    {
        _renderer.RenderError(message);
        Console.WriteLine();
        Console.WriteLine("  Press any key to continue...");
        Console.ReadKey();
    }

    private int? PromptFloor(string prompt, int min, int max)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();

        if (!int.TryParse(input, out var value))
        {
            _renderer.RenderError($"'{input}' is not a valid number.");
            Console.WriteLine("  Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        if (value < min || value > max)
        {
            _renderer.RenderError($"Floor {value} is out of range. Please enter a floor between {min} and {max}.");
            Console.WriteLine("  Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        return value;
    }
}
