public interface IElevatorController
{
    void RequestElevator(int floor, int passengerCount, int destinationFloor);
    void ArriveAtFloor(int floor);
    IEnumerable<ElevatorStatus> GetStatuses();
}
