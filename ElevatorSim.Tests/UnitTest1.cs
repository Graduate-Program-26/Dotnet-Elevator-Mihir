namespace ElevatorSim.Tests;

public class DomainUnitTests
{
    [Fact]
    public void MoveToFloor_SetsDirectionUp_WhenTargetIsHigher()
    {
        var elevator = new PassengerElevator(capacity: 10, startFloor: 1);
        elevator.MoveToFloor(5);
        Assert.Equal(ElevatorDirection.Up, elevator.Direction);
    }
}
