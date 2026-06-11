public class InvalidElevatorOperationException : Exception
{
    public string Operation { get; }

    public InvalidElevatorOperationException(string operation)
        : base($"The current elevator operation: {operation} cannot be performed.")
    {
        Operation = operation;
    }

    public InvalidElevatorOperationException(string operation, Exception innerException)
        : base($"The current elevator operation: {operation} cannot be performed.", innerException)
    {
        Operation = operation;
    }
}