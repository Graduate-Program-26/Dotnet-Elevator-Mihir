public class NearestAvailableDispatchStrategy : IDispatchStrategy
{
    public IElevator? SelectElevator(IEnumerable<IElevator> elevators, int requestedFloor, int passengerCount)
    {
        return elevators
            .OrderBy(e => Math.Abs(e.CurrentFloor - requestedFloor))
            .FirstOrDefault();
    }
}