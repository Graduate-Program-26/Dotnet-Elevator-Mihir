using ElevatorSim.Domain.Base;

namespace ElevatorSim.Application.Elevators;

public class PassengerElevator(int capacity = PassengerElevator.DefaultCapacity, int startFloor = 1) : ElevatorBase(capacity, DefaultSpeed, startFloor)
{
    public const int DefaultCapacity = 10;
    public const int DefaultSpeed = 1;
}
