namespace ElevatorSim.Application.Queue;

/// <summary>
/// Represents a queue of passenger requests for the elevator system.
/// </summary>
public class PassengerQueue
{
    private readonly Queue<(int Floor, int PassengerCount)> _queue = new();

    public int Count => _queue.Count;

    /// <summary>
    /// Adds a new passenger request to the queue.
    /// </summary>
    /// <param name="floor">The floor from which the passengers are requesting an elevator.</param>
    /// <param name="passengerCount">The number of passengers requesting the elevator.</param>
    public void Enqueue(int floor, int passengerCount)
    {
        _queue.Enqueue((floor, passengerCount));
    }

    /// <summary>
    /// Removes and returns the next passenger request from the queue.
    /// </summary>
    /// <returns>The next passenger request, or null if the queue is empty.</returns>
    public (int Floor, int PassengerCount)? Dequeue()
    {
        return _queue.TryDequeue(out var request) ? request : null;
    }

    /// <summary>
    /// Gets a value indicating whether the queue has any pending requests.
    /// </summary>
    public bool HasPendingRequests => _queue.Count > 0;
}
