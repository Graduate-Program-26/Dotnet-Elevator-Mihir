public class ConsoleRenderer : IConsoleRenderer
{
    private readonly List<string> _messages = [];

    public void RenderStatus(IEnumerable<ElevatorStatus> statuses)
    {
        Console.Clear();

        RenderTable(statuses);
        Console.WriteLine();
        RenderMessageLog();
    }

    public void RenderMessage(string message)
    {
        _messages.Add(message);
    }

    public void RenderError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ! {message}");
        Console.ResetColor();
    }

    private void RenderTable(IEnumerable<ElevatorStatus> statuses)
    {
        Console.WriteLine("╔══════════╦═══════╦═══════════════╦══════════╦═════════════╗");
        Console.WriteLine("║ Elevator ║ Floor ║   Direction   ║  State   ║  Passengers ║");
        Console.WriteLine("╠══════════╬═══════╬═══════════════╬══════════╬═════════════╣");

        var statusList = statuses.ToList();
        for (int i = 0; i < statusList.Count; i++)
        {
            var status = statusList[i];
            var elevatorNum = $"#{i + 1}".PadLeft(4).PadRight(8);
            var floor = status.CurrentFloor.ToString().PadLeft(3).PadRight(5);
            var passengers = $"{status.PassengerCount} / {status.Capacity}".PadLeft(6).PadRight(11);

            Console.Write($"║ {elevatorNum} ║ {floor} ║ ");

            RenderDirection(status.Direction);

            Console.Write(" ║ ");

            RenderState(status.State);

            Console.WriteLine($" ║ {passengers} ║");
        }
        Console.WriteLine("╚══════════╩═══════╩═══════════════╩══════════╩═════════════╝");
    }

    private void RenderDirection(ElevatorDirection direction)
    {
        var (colour, label) = direction switch
        {
            ElevatorDirection.Up => (ConsoleColor.Green, "      Up     "),
            ElevatorDirection.Down => (ConsoleColor.Red, "     Down    "),
            ElevatorDirection.Stationary => (ConsoleColor.Gray, "  Stationary "),
            _ => (ConsoleColor.White, "    Unknown    ")
        };

        Console.ForegroundColor = colour;
        Console.Write(label);
        Console.ResetColor();
    }

    private void RenderState(ElevatorState state)
    {
        var (colour, label) = state switch
        {
            ElevatorState.Idle => (ConsoleColor.Gray, "  Idle  "),
            ElevatorState.Moving => (ConsoleColor.Yellow, " Moving "),
            ElevatorState.DoorsOpen => (ConsoleColor.Cyan, "Doors Open"),
            _ => (ConsoleColor.White, " Unknown")
        };

        Console.ForegroundColor = colour;
        Console.Write(label);
        Console.ResetColor();
    }

    private void RenderMessageLog()
    {
        Console.WriteLine("  Recent activity:");
        Console.WriteLine("  ─────────────────────────────────────────");

        if (_messages.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  No activity yet.");
            Console.ResetColor();
            return;
        }

        foreach (var message in _messages)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  > ");
            Console.ResetColor();
            Console.WriteLine(message);
        }
    }
}
