namespace ElevatorSim.Infrastructure.Configuration;

/// <summary>
/// A record that encapsulates the configuration settings for the elevator simulation.
/// </summary>
/// <param name="NumberOfFloors">The number of floors in the building.</param>
/// <param name="NumberOfElevators">The number of elevators in the simulation.</param>
/// <param name="ElevatorCapacity">The maximum number of passengers each elevator can carry.</param>
/// <param name="NumberOfFreightElevators">The number of freight elevators in the simulation.</param>
/// <param name="NumberOfHighSpeedElevators">The number of high-speed elevators in the simulation.</param>
public record SimulationConfig(
    int NumberOfFloors,
    int NumberOfElevators,
    int ElevatorCapacity,
    int NumberOfFreightElevators = 0,
    int NumberOfHighSpeedElevators = 0
);
