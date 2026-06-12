public class NearestAvailableDispatchStrategy : IDispatchStrategy
{
    public IElevator? SelectElevator(IEnumerable<IElevator> elevators, int requestedFloor, int passengerCount)
    {
        return elevators
            .Where(elevator => elevator.CanAcceptPassengers)
            .Where(elevator => elevator.State != ElevatorState.DoorsOpen)
            .OrderBy(elevator => Math.Abs(elevator.CurrentFloor - requestedFloor))
            .FirstOrDefault();
    }
}
