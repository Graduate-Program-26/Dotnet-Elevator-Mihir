public class PassengerElevator : ElevatorBase
{
    public const int DefaultCapacity = 10;
    public const int DefaultSpeed = 1;

    public PassengerElevator(int capacity = DefaultCapacity, int startFloor = 1) : base(capacity, DefaultSpeed, startFloor) { }
}
