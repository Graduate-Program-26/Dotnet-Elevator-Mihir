using ElevatorSim.Domain.Models;

namespace ElevatorSim.Domain.Interfaces;

public interface IDispatchStrategy
{
    IElevator? SelectElevator(IEnumerable<IElevator> elevators, int requestedFloor, IEnumerable<Passenger> passengers);
}
