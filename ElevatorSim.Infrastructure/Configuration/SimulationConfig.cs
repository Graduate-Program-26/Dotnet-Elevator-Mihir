namespace ElevatorSim.Infrastructure.Configuration;

public record SimulationConfig(
    int NumberOfFloors,
    int NumberOfElevators,
    int ElevatorCapacity);
