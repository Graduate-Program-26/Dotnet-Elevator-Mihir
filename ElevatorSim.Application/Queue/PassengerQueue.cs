public class PassengerQueue
{
    private readonly Queue<(int Floor, int PassengerCount)> _queue = new();

    public int Count => _queue.Count;

    public void Enqueue(int floor, int passengerCount)
    {
        _queue.Enqueue((floor, passengerCount));
    }

    public (int Floor, int PassengerCount)? Dequeue()
    {
        return _queue.TryDequeue(out var request) ? request : null;
    }

    public bool HasPendingRequests => _queue.Count > 0;
}
