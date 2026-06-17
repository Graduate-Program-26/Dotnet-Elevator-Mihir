using ElevatorSim.Application.Queue;
using ElevatorSim.Application.Utilities;

namespace ElevatorSim.Application.Controllers;

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
        {
            throw new InvalidFloorException(floor);
        }

        var passengerList = passengers.ToList();

        if (passengerList.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(passengers), "At least one passenger is required.");
        }

        var invalidDestination = passengerList
            .FirstOrDefault(passenger =>
            passenger.DestinationFloor < MinFloor ||
            passenger.DestinationFloor > MaxFloor);

        if (invalidDestination is not null)
        {
            throw new InvalidFloorException(invalidDestination.DestinationFloor);
        }

        var sameFloor = passengerList
            .FirstOrDefault(p => p.DestinationFloor == floor);

        if (sameFloor is not null)
        {
            throw new InvalidElevatorOperationException($"Passenger destination floor {sameFloor.DestinationFloor} " + $"is the same as the origin floor.");
        }

        var selectedElevators = DetermineElevatorsNeeded(floor, passengerList);

        if (selectedElevators.Count == 0)
        {
            foreach (var _ in passengerList)
            {
                _pendingRequests.Enqueue(floor, 1);
            }

            OnElevatorMoved?.Invoke("No elevators available. Your request has been queued.");
            return;
        }

        var assignments = PassengerDistributor.Distribute(passengerList, selectedElevators, floor);

        foreach (var (elevator, assigned) in assignments)
        {
            if (assigned.Count == 0)
                continue;

            var available = elevator.Capacity - elevator.PassengerCount;
            var toBoard = assigned.Take(available).ToList();

            foreach (var passenger in toBoard)
                elevator.BoardPassenger(passenger);

            DeliverPassengers(elevator, floor, toBoard);
        }
    }

    private List<IElevator> DetermineElevatorsNeeded(int floor, List<Passenger> passengers)
    {
        var selected = new List<IElevator>();
        var remainingCount = passengers.Count;

        var naturalGroupCount = PassengerGrouper
            .Group(passengers, _elevators.Count())
            .Count;

        while (remainingCount > 0 || selected.Count < naturalGroupCount)
        {
            var available = _elevators.Except(selected);

            var elevator = _dispatchStrategy.SelectElevator(
                available, floor, passengers);

            if (elevator is null) break;

            selected.Add(elevator);
            remainingCount -= elevator.Capacity - elevator.PassengerCount;

            if (selected.Count >= naturalGroupCount && remainingCount <= 0)
                break;
        }
        return selected;
    }

    private void DeliverPassengers(IElevator elevator, int originFloor, List<Passenger> boarded)
    {
        var elevatorIndex = _elevators.ToList().IndexOf(elevator) + 1;

        elevator.MoveToFloor(originFloor);

        OnElevatorMoved?.Invoke($"\e[36mElevator #{elevatorIndex} dispatched to floor {originFloor}. {boarded.Count} passenger(s) is/are on board.\e[0m");

        var destinations = DestinationSorter
            .SortByProximity(originFloor, boarded)
            .ToList();

        foreach (var destination in destinations)
        {
            var dropOffCount = boarded
                .Count(p => p.DestinationFloor == destination);

            elevator.MoveToFloor(destination);
            elevator.DeboardPassengers();

            OnElevatorMoved?.Invoke($"\e[32mElevator #{elevatorIndex} arrived at floor {destination}. {dropOffCount} passenger(s) dropped off.\e[0m");
        }
    }
}
