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
    .BuildServiceProvider();

var controller = services.GetRequiredService<IElevatorController>();

var statuses = controller.GetStatuses();

Console.WriteLine($"Simulation started with {statuses.Count()} elevators:");
Console.WriteLine();

foreach (var (status, index) in statuses.Select((s, i) => (s, i)))
{
    Console.WriteLine($"Elevator #{index + 1}");
    Console.WriteLine($"  Floor:      {status.CurrentFloor}");
    Console.WriteLine($"  Direction:  {status.Direction}");
    Console.WriteLine($"  State:      {status.State}");
    Console.WriteLine($"  Passengers: {status.PassengerCount} / {status.Capacity}");
    Console.WriteLine();
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();