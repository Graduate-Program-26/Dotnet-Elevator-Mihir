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
}