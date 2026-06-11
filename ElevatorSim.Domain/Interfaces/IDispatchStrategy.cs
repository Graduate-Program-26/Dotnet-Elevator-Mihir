public interface IDispatchStrategy
{
    IElevator? SelectElevator(IEnumerable<IElevator> elevators, int requestedFloor, int passengerCount);
}
