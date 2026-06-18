using ElevatorSim.Domain.Models;

namespace ElevatorSim.Domain.Interfaces;

/// <summary>
/// Defines a strategy for selecting an elevator to respond to a call request. This interface allows for different algorithms to be implemented for elevator dispatching.
/// </summary>
public interface IDispatchStrategy
{
    IElevator? SelectElevator(IEnumerable<IElevator> elevators, int requestedFloor, IEnumerable<Passenger> passengers);
}
