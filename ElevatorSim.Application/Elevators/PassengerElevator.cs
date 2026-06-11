public class PassengerElevator : ElevatorBase
{
    public const int DefaultCapacity = 10;

    public PassengerElevator(int capacity = DefaultCapacity, int startFloor = 1)
        : base(capacity, startFloor) { }
}