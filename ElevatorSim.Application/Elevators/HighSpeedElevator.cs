public class HighSpeedElevator : ElevatorBase
{
    public const int DefaultCapacity = 6;
    public const int DefaultSpeed = 3;

    public HighSpeedElevator(int capacity = DefaultCapacity, int startFloor = 1) : base(capacity, DefaultSpeed, startFloor) { }

    public override void MoveToFloor(int floor)
    {
        // @TODO: This is just a stub right now to pass the tests but actual logic has to be added
        base.MoveToFloor(floor);
    }
}
