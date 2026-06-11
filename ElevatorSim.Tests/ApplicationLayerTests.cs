namespace ElevatorSim.Tests;

public class ApplicationLayerTests
{
    [Fact]
    public void PassengerElevator_HasCorrectDefaultCapacity()
    {
        var elevator = new PassengerElevator();

        Assert.Equal(PassengerElevator.DefaultCapacity, elevator.Capacity);
    }


}