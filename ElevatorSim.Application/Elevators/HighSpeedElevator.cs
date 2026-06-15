public class HighSpeedElevator(int capacity = HighSpeedElevator.DefaultCapacity, int startFloor = 1) : ElevatorBase(capacity, DefaultSpeed, startFloor)
{
    public const int DefaultCapacity = 6;
    public const int DefaultSpeed = 3;

    public override void MoveToFloor(int floor)
    {
        // @TODO: This is just a stub right now to pass the tests but actual logic has to be added
        base.MoveToFloor(floor);
    }
}
