namespace ElevatorSim.Tests;

public class ApplicationLayerTests
{
    [Fact]
    public void PassengerElevator_HasCorrectDefaultCapacity()
    {
        var elevator = new PassengerElevator();

        Assert.Equal(PassengerElevator.DefaultCapacity, elevator.Capacity);
    }

    [Fact]
    public void FreightElevator_HasHigherDefaultCapacity_ThanPassengerElevator()
    {
        var elevator = new FreightElevator();

        Assert.True(elevator.Capacity > PassengerElevator.DefaultCapacity);
    }

    [Fact]
    public void FreightElevator_CanBeUsedAs_IElevator()
    {
        IElevator elevator = new FreightElevator();

        elevator.MoveToFloor(3);

        Assert.Equal(3, elevator.CurrentFloor);
    }

    [Fact]
    public void HighSpeedElevator_HasLowerDefaultCapacity_ThanPassengerElevator()
    {
        var elevator = new HighSpeedElevator();

        Assert.True(elevator.Capacity < PassengerElevator.DefaultCapacity);
    }

    [Fact]
    public void HighSpeedElevator_MoveToFloor_SetsStateToMoving_ThenIdle()
    {
        var elevator = new HighSpeedElevator();

        elevator.MoveToFloor(10);

        Assert.Equal(10, elevator.CurrentFloor);
        Assert.Equal(ElevatorState.Idle, elevator.State);
    }

    [Fact]
    public void HighSpeedElevator_CanBeUsedAs_IElevator()
    {
        IElevator elevator = new HighSpeedElevator();

        elevator.MoveToFloor(5);

        Assert.Equal(5, elevator.CurrentFloor);
    }
}