public interface IElevatorController
{
    void RequestElevator(int floor, int passengerCount);
    IEnumerable<ElevatorStatus> GetStatuses();
}
