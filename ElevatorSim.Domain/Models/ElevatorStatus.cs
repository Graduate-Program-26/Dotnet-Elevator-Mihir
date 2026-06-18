namespace ElevatorSim.Domain.Models;

/// <summary>
/// Represents the current status of an elevator.
/// </summary>
/// <param name="ElevatorId">Represents the unique identifier for the elevator.</param>
/// <param name="CurrentFloor">Represents the floor where the elevator is currently located.</param>
/// <param name="Direction">Represents the direction that the elevator is moving.</param>
/// <param name="State">Represents the current state of the elevator.</param>
/// <param name="PassengerCount">Represents the number of passengers in the lift currently.</param>
/// <param name="Capacity">Represents the maximum number of passengers the lift can hold.</param>
public record ElevatorStatus(
    int ElevatorId,
    int CurrentFloor,
    ElevatorDirection Direction,
    ElevatorState State,
    int PassengerCount,
    int Capacity
);
