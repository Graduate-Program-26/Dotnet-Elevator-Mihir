namespace ElevatorSim.Domain.Exceptions;

/// <summary>
/// A custom exception that gets thrown when the maximum capacity of the elevator has been exceeded.
/// </summary>
public class CapacityExceededException : Exception
{
    public int Capacity { get; }

    public CapacityExceededException(int capacity)
        : base($"A maximum capacity of: {nameof(capacity)} has been exceeded.")
    {
        Capacity = capacity;
    }

    public CapacityExceededException(int capacity, Exception innerException)
        : base($"A maximum capacity of: {nameof(capacity)} has been exceeded.", innerException)
    {
        Capacity = capacity;
    }
}
