namespace ElevatorSim.Domain.Exceptions;

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
