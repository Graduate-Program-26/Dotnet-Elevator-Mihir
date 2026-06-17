using ElevatorSim.Application.Queue;
using ElevatorSim.Application.Utilities;
using ElevatorSim.Domain.Exceptions;
using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Domain.Models;

using Microsoft.Extensions.Logging;


namespace ElevatorSim.Application.Controllers;

public class ElevatorController(
    IEnumerable<IElevator> elevators,
    IDispatchStrategy dispatchStrategy,
    ILogger<ElevatorController> logger) : IElevatorController
{
    private readonly IEnumerable<IElevator> _elevators = elevators;
    private readonly IDispatchStrategy _dispatchStrategy = dispatchStrategy;
    private readonly ILogger<ElevatorController> _logger = logger;
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
            _logger.LogWarning("Invalid floor {Floor} requested — must be between {MinFloor} and {MaxFloor}.", floor, MinFloor, MaxFloor);
            throw new InvalidFloorException(floor);
        }

        var passengerList = passengers.ToList();

        _logger.LogInformation("Elevator requested for floor {Floor} with {PassengerCount} passenger(s).", floor, passengerList.Count);

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
            _logger.LogWarning("Rejecting request: Passenger has an invalid destination floor {DestinationFloor}.", invalidDestination.DestinationFloor);
            throw new InvalidFloorException(invalidDestination.DestinationFloor);
        }

        var sameFloor = passengerList
            .FirstOrDefault(p => p.DestinationFloor == floor);

        if (sameFloor is not null)
        {
            _logger.LogWarning("Rejecting request: Passenger destination floor {DestinationFloor} matches origin floor {Floor}.", sameFloor.DestinationFloor, floor);
            throw new InvalidElevatorOperationException($"Passenger destination floor {sameFloor.DestinationFloor} " + $"is the same as the origin floor.");
        }

        var selectedElevators = DetermineElevatorsNeeded(floor, passengerList);

        if (selectedElevators.Count == 0)
        {
            _logger.LogWarning("No elevators available for floor {Floor} — queuing {Count} passenger request(s).", floor, passengerList.Count);

            foreach (var _ in passengerList)
            {
                _pendingRequests.Enqueue(floor, 1);
            }

            OnElevatorMoved?.Invoke("No elevators available. Your request has been queued.");
            return;
        }

        _logger.LogInformation("Dispatching {ElevatorCount} elevator(s) to floor {Floor}.", selectedElevators.Count, floor);

        var assignments = PassengerDistributor.Distribute(passengerList, selectedElevators, floor);

        foreach (var (elevator, assigned) in assignments)
        {
            if (assigned.Count == 0)
                continue;

            var available = elevator.Capacity - elevator.PassengerCount;
            var toBoard = assigned.Take(available).ToList();

            _logger.LogInformation("Elevator boarding {BoardingCount} of {AssignedCount} assigned passengers at floor {Floor}. (Remaining capacity: {Capacity})",
                toBoard.Count, assigned.Count, floor, available - toBoard.Count);

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

        OnElevatorMoved?.Invoke($"\e[36mElevator #{elevatorIndex} ({elevator.GetType().Name}) dispatched to floor {originFloor}. {boarded.Count} passenger(s) is/are on board.\e[0m");

        var destinations = DestinationSorter
            .SortByProximity(originFloor, boarded)
            .ToList();

        foreach (var destination in destinations)
        {
            var dropOffCount = boarded
                .Count(p => p.DestinationFloor == destination);

            elevator.MoveToFloor(destination);
            elevator.DeboardPassengers();

            OnElevatorMoved?.Invoke($"\e[32mElevator #{elevatorIndex} ({elevator.GetType().Name}) arrived at floor {destination}. {dropOffCount} passenger(s) dropped off.\e[0m");
        }
    }
}
