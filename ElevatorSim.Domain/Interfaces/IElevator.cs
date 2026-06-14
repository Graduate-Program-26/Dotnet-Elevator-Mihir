public interface IElevator
{
    int CurrentFloor { get; }
    ElevatorDirection Direction { get; }
    ElevatorState State { get; }
    int PassengerCount { get; }
    int Capacity { get; }
    bool CanAcceptPassengers { get; }
    IReadOnlyList<int> DestinationFloors { get; }

    void MoveToFloor(int floor);
    void AddPassengers(int count);
    void RemovePassengers(int count);
    void BoardPassenger(Passenger passenger);
    void DeboardPassengers();
    event Action<int>? OnArrival;
}
