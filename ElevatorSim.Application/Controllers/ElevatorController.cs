public class ElevatorController(
    IEnumerable<IElevator> elevators,
    IDispatchStrategy dispatchStrategy) : IElevatorController
{
    private readonly IEnumerable<IElevator> _elevators = elevators;
    private readonly IDispatchStrategy _dispatchStrategy = dispatchStrategy;
    private readonly PassengerQueue _pendingRequests = new();

    public const int MinFloor = 1;
    public const int MaxFloor = 20;
    public int PendingRequestCount => _pendingRequests.Count;

    public IEnumerable<ElevatorStatus> GetStatuses()
    {
        return _elevators.Select((e, index) => new ElevatorStatus(
            index + 1,
            e.CurrentFloor,
            e.Direction,
            e.State,
            e.PassengerCount,
            e.Capacity));
    }

    public void RequestElevator(int floor, int passengerCount)
    {
        if (floor < MinFloor || floor > MaxFloor)
            throw new InvalidFloorException(floor);

        if (passengerCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(passengerCount), "Passenger count must be greater than zero.");

        var remaining = passengerCount;

        while (remaining > 0)
        {
            var elevator = _dispatchStrategy.SelectElevator(
                _elevators, floor, remaining);

            if (elevator is null)
            {
                _pendingRequests.Enqueue(floor, remaining);
                return;
            }

            var available = elevator.Capacity - elevator.PassengerCount;
            var boarding = Math.Min(remaining, available);

            elevator.MoveToFloor(floor);
            elevator.AddPassengers(boarding);

            remaining -= boarding;
        }
    }
}
