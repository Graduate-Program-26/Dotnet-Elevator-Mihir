using ElevatorSim.Domain.Base;

namespace ElevatorSim.Application.Elevators;

/// <summary>
/// A concrete implementation of an elevator designed for high-speed travel.
/// </summary>
/// <param name="capacity">The maximum number of passengers the elevator can carry.</param>
/// <param name="startFloor">The floor where the elevator starts.</param>
public class HighSpeedElevator(int capacity = HighSpeedElevator.DefaultCapacity, int startFloor = 1) : ElevatorBase(capacity, DefaultSpeed, startFloor)
{
    public const int DefaultCapacity = 6;
    public const int DefaultSpeed = 3;
}
