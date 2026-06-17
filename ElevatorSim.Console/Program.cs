using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

var config = new SimulationConfig(
    NumberOfFloors: 20,
    NumberOfElevators: 3,
    ElevatorCapacity: 10);

var services = new ServiceCollection()
    .AddLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSerilog(new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger());
    })
    .AddElevatorSimulation(config)
    .AddSingleton<IConsoleRenderer, ConsoleRenderer>()
    .BuildServiceProvider();

var controller = services.GetRequiredService<IElevatorController>();
var renderer = services.GetRequiredService<IConsoleRenderer>();
var inputHandler = new ConsoleInputHandler(controller, renderer);

controller.OnElevatorMoved += renderer.RenderMessage;

var running = true;

while (running)
{
    renderer.RenderStatus(controller.GetStatuses());

    Console.WriteLine();
    Console.WriteLine("  [1] Call elevator   [2] View status   [Q] Quit");
    Console.Write("  > ");

    var key = Console.ReadLine()?.Trim().ToUpperInvariant();

    switch (key)
    {
        case "1":
            inputHandler.HandleCallElevator(config.NumberOfFloors);
            break;

        case "2":
            // this case just re-renders the status for now
            break;

        case "Q":
            running = false;
            break;

        default:
            renderer.RenderError(
                $"'{key}' is not a valid option. Please choose 1, 2, or Q.");
            Console.WriteLine("  Press any key to continue...");
            Console.ReadKey();
            break;
    }
}

Console.Clear();
