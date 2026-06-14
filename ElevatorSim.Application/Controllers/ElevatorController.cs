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
    public event Action<string>? OnElevatorMoved;

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

    public void RequestElevator(int floor, IEnumerable<Passenger> passengers)
    {
        if (floor < MinFloor || floor > MaxFloor)
            throw new InvalidFloorException(floor);

        var passengerList = passengers.ToList();

        if (passengerList.Count == 0)
            throw new ArgumentOutOfRangeException(
                nameof(passengers),
                "At least one passenger is required.");

        var remaining = new Queue<Passenger>(passengerList);

        while (remaining.Count > 0)
        {
            var elevator = _dispatchStrategy.SelectElevator(
                _elevators, floor, remaining.Count);

            if (elevator is null)
            {
                foreach (var p in remaining)
                    _pendingRequests.Enqueue(floor, 1);
                return;
            }

            var boarded = new List<Passenger>();
            while (remaining.Count > 0 && elevator.CanAcceptPassengers)
            {
                var passenger = remaining.Dequeue();
                elevator.BoardPassenger(passenger);
                boarded.Add(passenger);
            }

            elevator.MoveToFloor(floor);

            var destinations = boarded
                .Select(p => p.DestinationFloor)
                .Distinct()
                .OrderBy(f => Math.Abs(f - floor));

            var elevatorIndex = _elevators.ToList().IndexOf(elevator) + 1;

            foreach (var destination in destinations)
            {
                var passengersAtDestination = boarded
                    .Count(p => p.DestinationFloor == destination);

                elevator.MoveToFloor(destination);
                elevator.DeboardPassengers();

                OnElevatorMoved?.Invoke(
                    $"Elevator #{elevatorIndex} arrived at floor {destination}. {passengersAtDestination} passenger(s) dropped off.");
            }
        }
    }

    public void ArriveAtFloor(int floor)
    {
        foreach (var elevator in _elevators.Where(elev => elev.CurrentFloor == floor))
        {
            elevator.DeboardPassengers();
        }
    }
}
