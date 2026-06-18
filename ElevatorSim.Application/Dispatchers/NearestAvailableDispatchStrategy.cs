using ElevatorSim.Application.Utilities;
using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Domain.Models;

using Microsoft.Extensions.Logging;

namespace ElevatorSim.Application.Dispatchers;

/// <summary>
/// A concrete implementation of the IDispatchStrategy interface that selects the nearest available elevator based on the trip cost calculated for each elevator.
/// </summary>
public class NearestAvailableDispatchStrategy(ILogger<NearestAvailableDispatchStrategy> logger) : IDispatchStrategy
{
    private readonly ILogger<NearestAvailableDispatchStrategy> _logger = logger;

    /// <summary>
    /// Selects the nearest available elevator based on the trip cost calculated for each elevator. The elevator with the lowest trip cost is selected,
    /// provided it can accept passengers and is not currently in the "DoorsOpen" state. Null is returned is no suitable elevator is found.
    /// </summary>
    /// <param name="elevators">The collection of elevators in the building.</param>
    /// <param name="requestedFloor">The floor where the elevator is being requested.</param>
    /// <param name="passengers">The list of passengers requesting an elevator.</param>
    /// <returns>The selected elevator, or null if no suitable elevator is found.</returns>
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
