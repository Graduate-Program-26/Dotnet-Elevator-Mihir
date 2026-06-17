namespace ElevatorSim.Infrastructure.Configuration;

public record SimulationConfig(
    int NumberOfFloors,
    int NumberOfElevators,
    int ElevatorCapacity,
    int NumberOfFreightElevators = 0,
    int NumberOfHighSpeedElevators = 0
);
