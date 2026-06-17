using ElevatorSim.Application.Utilities;
using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Domain.Models;

using Microsoft.Extensions.Logging;

namespace ElevatorSim.Application.Dispatchers;

public class NearestAvailableDispatchStrategy(ILogger<NearestAvailableDispatchStrategy> logger) : IDispatchStrategy
{
    private readonly ILogger<NearestAvailableDispatchStrategy> _logger = logger;

    public IElevator? SelectElevator(IEnumerable<IElevator> elevators, int requestedFloor, IEnumerable<Passenger> passengers)
    {
        var passengerList = passengers.ToList();

        var selected = elevators
            .Where(elevator => elevator.CanAcceptPassengers)
            .Where(elevator => elevator.State != ElevatorState.DoorsOpen)
            .OrderBy(elevator => TripCostCalculator.Calculate(elevator, requestedFloor, passengerList))
            .FirstOrDefault();

        _logger.LogDebug("Dispatch strategy selected elevator at floor {ElevatorFloor} for request at floor {RequestedFloor}.", selected?.CurrentFloor, requestedFloor);
        return selected;
    }
}
