namespace ElevatorSim.Domain.Exceptions;

/// <summary>
/// A custom exception that gets thrown when an invalid floor number is provided to the elevator.
/// </summary>
public class InvalidFloorException : Exception
{
    public int FloorNumber { get; }

    public InvalidFloorException(int floorNumber)
        : base($"Floor number: {nameof(floorNumber)} is invalid")
    {
        FloorNumber = floorNumber;
    }

    public InvalidFloorException(int floorNumber, Exception innerException)
        : base($"Floor number: {nameof(floorNumber)} is invalid. ", innerException)
    {
        FloorNumber = floorNumber;
    }
}
