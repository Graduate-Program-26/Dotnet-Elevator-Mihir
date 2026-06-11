public class InvalidFloorException : Exception
{
    public int FloorNumber { get; }

    public InvalidFloorException(int floorNumber)
        : base($"Floor number: {floorNumber} is invalid")
    {
        FloorNumber = floorNumber;
    }

    public InvalidFloorException(int floorNumber, Exception innerException)
        : base($"Floor number: {floorNumber} is invalid. ", innerException)
    {
        FloorNumber = floorNumber;
    }
}