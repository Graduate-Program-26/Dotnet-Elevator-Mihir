namespace ElevatorSim.Domain.Exceptions;

/// <summary>
/// A custome exception that gets thrown when an invalid operation is performed on the elevator.
/// </summary>
public class InvalidElevatorOperationException : Exception
{
    public string Operation { get; }

    public InvalidElevatorOperationException(string operation)
        : base($"The current elevator operation: {nameof(operation)} cannot be performed.")
    {
        Operation = operation;
    }

    public InvalidElevatorOperationException(string operation, Exception innerException)
        : base($"The current elevator operation: {nameof(operation)} cannot be performed.", innerException)
    {
        Operation = operation;
    }
}
