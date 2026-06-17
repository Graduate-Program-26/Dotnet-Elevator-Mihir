using ElevatorSim.Domain.Models;

namespace ElevatorSim.Domain.Interfaces;

public interface IElevatorController
{
    void RequestElevator(int floor, IEnumerable<Passenger> passengers);
    IEnumerable<ElevatorStatus> GetStatuses();
    event Action<string>? OnElevatorMoved;
}
