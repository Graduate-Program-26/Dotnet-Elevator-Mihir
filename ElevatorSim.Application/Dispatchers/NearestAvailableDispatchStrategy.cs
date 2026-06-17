using ElevatorSim.Application.Utilities;

namespace ElevatorSim.Application.Dispatchers;

public class NearestAvailableDispatchStrategy : IDispatchStrategy
{
    public IElevator? SelectElevator(IEnumerable<IElevator> elevators, int requestedFloor, IEnumerable<Passenger> passengers)
    {
        var passengerList = passengers.ToList();

        return elevators
            .Where(elevator => elevator.CanAcceptPassengers)
            .Where(elevator => elevator.State != ElevatorState.DoorsOpen)
            .OrderBy(elevator => TripCostCalculator.Calculate(elevator, requestedFloor, passengerList))
            .FirstOrDefault();
    }
}
