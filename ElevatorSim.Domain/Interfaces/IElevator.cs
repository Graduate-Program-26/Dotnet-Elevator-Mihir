public interface IElevator
{
    int CurrentFloor { get; set; }
    ElevatorDirection Direction { get; set; }
    ElevatorState State { get; set; }
    int PassengerCount { get; set; }
    int Capacity { get; set; }
    bool CanAcceptPassengers { get; set; }

    void MoveToFloor(int floor);
    void AddPassengers(int count);
    void RemovePassengers(int count);
}