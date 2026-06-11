public interface IElevator
{
    int CurrentFloor { get; }
    ElevatorDirection Direction { get; }
    ElevatorState State { get; }
    int PassengerCount { get; }
    int Capacity { get; }
    bool CanAcceptPassengers { get; }

    void MoveToFloor(int floor);
    void AddPassengers(int count);
    void RemovePassengers(int count);
}
