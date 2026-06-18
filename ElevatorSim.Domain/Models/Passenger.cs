namespace ElevatorSim.Domain.Models;

/// <summary>
/// Represents a passenger waiting to use an elevator and where they want to go. 
/// </summary>
/// <param name="StartingFloor">Represents the floor where the passenger is waiting.</param>
/// <param name="DestinationFloor">Represents the floor where the passenger wants to go.</param>
public record Passenger(int StartingFloor, int DestinationFloor);
