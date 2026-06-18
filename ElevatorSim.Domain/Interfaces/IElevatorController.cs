using ElevatorSim.Domain.Models;

namespace ElevatorSim.Domain.Interfaces;

/// <summary>
/// Defines the interface for the elevator controller, which manages elevator requests and coordinates elevator movements.
/// </summary>
public interface IElevatorController
{
    void RequestElevator(int floor, IEnumerable<Passenger> passengers);
    IEnumerable<ElevatorStatus> GetStatuses();
    event Action<string>? OnElevatorMoved;
}
