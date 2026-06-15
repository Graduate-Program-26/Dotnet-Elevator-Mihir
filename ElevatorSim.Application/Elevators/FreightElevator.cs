public class FreightElevator : ElevatorBase
{
    public const int DefaultCapacity = 20;
    public const int DefaultSpeed = 2;

    public FreightElevator(int capacity = DefaultCapacity, int startFloor = 1) : base(capacity, DefaultSpeed, startFloor) { }
}