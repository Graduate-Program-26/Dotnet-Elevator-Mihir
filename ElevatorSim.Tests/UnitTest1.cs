using Moq;

using Timer = System.Timers.Timer;

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

    [Fact]
    public void MoveToFloor_SetsDirectionDown_WhenTargetIsLower()
    {
        var elevator = new PassengerElevator(capacity: 10, startFloor: 8);
        elevator.MoveToFloor(3);
        Assert.Equal(ElevatorDirection.Down, elevator.Direction);
    }

    [Fact]
    public void RequestElevator_UsesDispatchStrategy_ToSelectElevator()
    {
        var mockStrategy = new Mock<IDispatchStrategy>();
        var controller = new ElevatorController(elevators, mockStrategy.Object);

        controller.RequestElevator(5, 2);

        mockStrategy.Verify(s => s.SelectElevator(
            It.IsAny<IEnumerable<IElevator>>(), 5, 2), Timer.Once);
    }

    [Fact]
    public void GetStatuses_ReturnsStatusForAllElevators()
    {
        var elevator = new PassengerElevator(capacity: 10, startFloor: 1);
        var controller = new ElevatorController([elevator]);

        var statuses = controller.GetStatuses();

        Assert.Single(statuses);
        Assert.Equal(1, statuses.First().CurrentFloor);
        Assert.Equal(ElevatorDirection.Stationary, statuses.First().Direction);
        Assert.Equal(ElevatorState.Idle, statuses.First().State);
    }
}
