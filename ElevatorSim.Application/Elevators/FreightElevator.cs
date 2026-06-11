public class FreightElevator : ElevatorBase
{
    public const int DefaultCapacity = 20;

    public FreightElevator(int capacity = DefaultCapacity, int startFloor = 1)
        : base(capacity, startFloor) { }
}