namespace ElevatorSim.Application.Elevators;

public class HighSpeedElevator(int capacity = HighSpeedElevator.DefaultCapacity, int startFloor = 1) : ElevatorBase(capacity, DefaultSpeed, startFloor)
{
    public const int DefaultCapacity = 6;
    public const int DefaultSpeed = 3;
}
