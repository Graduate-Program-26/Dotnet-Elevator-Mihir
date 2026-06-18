using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Infrastructure.Configuration;
using ElevatorSim.Infrastructure.DependencyInjection;
using ElevatorSim.Inputs;
using ElevatorSim.Rendering;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

const string LogFilePath = "logs/elevator-sim.log";

var config = new SimulationConfig(
    NumberOfFloors: 20,
    NumberOfElevators: 1,
    ElevatorCapacity: 10,
    NumberOfFreightElevators: 1,
    NumberOfHighSpeedElevators: 1);

var services = new ServiceCollection()
    .AddLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSerilog(new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                LogFilePath,
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}")
            .CreateLogger());
    })
    .AddElevatorSimulation(config)
    .AddSingleton<IConsoleRenderer, ConsoleRenderer>()
    .BuildServiceProvider();

var controller = services.GetRequiredService<IElevatorController>();
var renderer = services.GetRequiredService<IConsoleRenderer>();
var inputHandler = new ConsoleInputHandler(controller, renderer, services.GetRequiredService<ILogger<ConsoleInputHandler>>());

controller.OnElevatorMoved += renderer.RenderMessage;

var running = true;
var logViewer = services.GetRequiredService<ILogViewer>();

while (running)
{
    renderer.RenderStatus(controller.GetStatuses());

    Console.WriteLine();
    Console.WriteLine("  [1] Call elevator   [2] View logs   [Q] Quit");
    Console.Write("  > ");

    var key = Console.ReadLine()?.Trim().ToUpperInvariant();

    switch (key)
    {
        case "1":
            inputHandler.HandleCallElevator(config.NumberOfFloors);
            break;

        case "2":
            var entries = logViewer.GetRecentEntries(30);
            renderer.RenderLogs(entries);
            Console.ReadKey();
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
