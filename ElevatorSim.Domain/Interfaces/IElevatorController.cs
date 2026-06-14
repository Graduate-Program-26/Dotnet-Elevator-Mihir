public interface IElevatorController
{
    void RequestElevator(int floor, IEnumerable<Passenger> passengers);
    void ArriveAtFloor(int floor);
    IEnumerable<ElevatorStatus> GetStatuses();
    event Action<string>? OnElevatorMoved;
}
