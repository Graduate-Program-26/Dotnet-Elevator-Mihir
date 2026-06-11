public class CapacityExceededException : Exception
{
    public int Capacity { get; }

    public CapacityExceededException(int capacity)
        : base($"A maxiumum capacity of: {capacity} has been exceeded.")
    {
        Capacity = capacity;
    }

    public CapacityExceededException(int capacity, Exception innerException)
        : base($"A maxiumum capacity of: {capacity} has been exceeded.", innerException)
    {
        Capacity = capacity;
    }
}