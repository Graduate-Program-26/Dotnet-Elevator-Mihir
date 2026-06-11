public record ElevatorStatus(
    int ElevatorId,
    int CurrentFloor,
    ElevatorDirection Direction,
    ElevatorState State,
    int PassengerCount,
    int Capacity
);