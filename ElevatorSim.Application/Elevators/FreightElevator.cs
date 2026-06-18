using ElevatorSim.Domain.Base;

namespace ElevatorSim.Application.Elevators;

/// <summary>
/// A concrete implementation of an elevator with a higher capacity than a passenger elevator.
/// </summary>
/// <param name="capacity">The maximum number of passengers the elevator can carry.</param>
/// <param name="startFloor">The floor where the elevator starts.</param>
public class FreightElevator(int capacity = FreightElevator.DefaultCapacity, int startFloor = 1) : ElevatorBase(capacity, DefaultSpeed, startFloor)
{
    public const int DefaultCapacity = 20;
    public const int DefaultSpeed = 2;
}
