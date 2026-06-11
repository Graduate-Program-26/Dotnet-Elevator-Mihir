using Moq;

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

    [Theory]
    [MemberData(nameof(AllElevatorTypes))]
    public void AllElevatorTypes_MoveToFloor_UpdatesCurrentFloor(IElevator elevator)
    {
        elevator.MoveToFloor(7);

        Assert.Equal(7, elevator.CurrentFloor);
    }

    [Theory]
    [MemberData(nameof(AllElevatorTypes))]
    public void AllElevatorTypes_AddPassengers_IncreasesPassengerCount(IElevator elevator)
    {
        elevator.AddPassengers(1);

        Assert.Equal(1, elevator.PassengerCount);
    }

    public static IEnumerable<object[]> AllElevatorTypes =>
    [
        [new PassengerElevator()],
        [new FreightElevator()],
        [new HighSpeedElevator()]
    ];

    [Fact]
    public void SelectElevator_ReturnsNearestAvailableElevator()
    {
        var elevatorOnFloor2 = new PassengerElevator(startFloor: 2);
        var elevatorOnFloor8 = new PassengerElevator(startFloor: 8);
        var strategy = new NearestAvailableDispatchStrategy();

        var selected = strategy.SelectElevator(
            [elevatorOnFloor2, elevatorOnFloor8],
            requestedFloor: 3,
            passengerCount: 1);

        Assert.Equal(elevatorOnFloor2, selected);
    }

    [Fact]
    public void SelectElevator_SkipsElevatorAtCapacity()
    {
        var fullElevator = new PassengerElevator(capacity: 2, startFloor: 1);
        fullElevator.AddPassengers(2);

        var availableElevator = new PassengerElevator(capacity: 10, startFloor: 8);
        var strategy = new NearestAvailableDispatchStrategy();

        var selected = strategy.SelectElevator(
            [fullElevator, availableElevator],
            requestedFloor: 1,
            passengerCount: 1);

        Assert.Equal(availableElevator, selected);
    }

    [Fact]
    public void SelectElevator_SkipsElevatorWithDoorsOpen()
    {
        var doorsOpenElevator = new PassengerElevator(capacity: 10, startFloor: 1);
        doorsOpenElevator.OpenDoors();

        var availableElevator = new PassengerElevator(capacity: 10, startFloor: 8);
        var strategy = new NearestAvailableDispatchStrategy();

        var selected = strategy.SelectElevator(
            [doorsOpenElevator, availableElevator],
            requestedFloor: 1,
            passengerCount: 1);

        Assert.Equal(availableElevator, selected);
    }

    [Fact]
    public void SelectElevator_ReturnsNull_WhenNoElevatorsAvailable()
    {
        var strategy = new NearestAvailableDispatchStrategy();

        var selected = strategy.SelectElevator(
            [],
            requestedFloor: 5,
            passengerCount: 1);

        Assert.Null(selected);
    }

    [Fact]
    public void SelectElevator_ReturnsNull_WhenAllElevatorsAtCapacity()
    {
        var fullElevator = new PassengerElevator(capacity: 2, startFloor: 1);
        fullElevator.AddPassengers(2);
        var strategy = new NearestAvailableDispatchStrategy();

        var selected = strategy.SelectElevator(
            [fullElevator],
            requestedFloor: 5,
            passengerCount: 1);

        Assert.Null(selected);
    }

    [Fact]
    public void SelectElevator_ReturnsFirst_WhenMultipleElevatorsEquidistant()
    {
        var elevatorOnFloor3 = new PassengerElevator(capacity: 10, startFloor: 3);
        var elevatorOnFloor7 = new PassengerElevator(capacity: 10, startFloor: 7);
        var strategy = new NearestAvailableDispatchStrategy();

        var selected = strategy.SelectElevator(
            [elevatorOnFloor3, elevatorOnFloor7],
            requestedFloor: 5,
            passengerCount: 1);

        Assert.NotNull(selected);
    }

    [Fact]
    public void GetStatuses_ReturnsStatusForAllElevators()
    {
        var elevator = new PassengerElevator(capacity: 10, startFloor: 1);
        var mockStrategy = new Mock<IDispatchStrategy>();
        var controller = new ElevatorController([elevator], mockStrategy.Object);

        var statuses = controller.GetStatuses();

        Assert.Single(statuses);
        Assert.Equal(1, statuses.First().CurrentFloor);
        Assert.Equal(ElevatorDirection.Stationary, statuses.First().Direction);
        Assert.Equal(ElevatorState.Idle, statuses.First().State);
        Assert.Equal(0, statuses.First().PassengerCount);
    }

    [Fact]
    public async Task RequestElevator_ThrowsInvalidFloorException_WhenFloorBelowMinimum()
    {
        var controller = new ElevatorController([], new Mock<IDispatchStrategy>().Object);

        await Assert.ThrowsAsync<InvalidFloorException>(
            () => controller.RequestElevator(0, 1));
    }

    [Fact]
    public async Task RequestElevator_ThrowsInvalidFloorException_WhenFloorAboveMaximum()
    {
        var controller = new ElevatorController([], new Mock<IDispatchStrategy>().Object);

        await Assert.ThrowsAsync<InvalidFloorException>(
            () => controller.RequestElevator(99, 1));
    }

    [Fact]
    public async Task RequestElevator_ThrowsArgumentException_WhenPassengerCountIsZero()
    {
        var controller = new ElevatorController([], new Mock<IDispatchStrategy>().Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => controller.RequestElevator(5, 0));
    }

    [Fact]
    public async Task RequestElevator_ThrowsArgumentException_WhenPassengerCountIsNegative()
    {
        var controller = new ElevatorController([], new Mock<IDispatchStrategy>().Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => controller.RequestElevator(5, -1));
    }

    [Fact]
    public void RequestElevator_DispatchesElevatorToRequestedFloor()
    {
        var mockElevator = new Mock<IElevator>();
        mockElevator.Setup(e => e.CanAcceptPassengers).Returns(true);
        mockElevator.Setup(e => e.State).Returns(ElevatorState.Idle);
        mockElevator.Setup(e => e.Capacity).Returns(10);
        mockElevator.Setup(e => e.PassengerCount).Returns(0);

        var mockStrategy = new Mock<IDispatchStrategy>();
        mockStrategy
            .Setup(s => s.SelectElevator(
                It.IsAny<IEnumerable<IElevator>>(), 5, 2))
            .Returns(mockElevator.Object);

        var controller = new ElevatorController([mockElevator.Object], mockStrategy.Object);

        controller.RequestElevator(5, 2);

        mockElevator.Verify(e => e.MoveToFloor(5), Times.Once);
        mockElevator.Verify(e => e.AddPassengers(2), Times.Once);
    }

    [Fact]
    public void RequestElevator_QueuesRequest_WhenNoElevatorAvailable()
    {
        var mockStrategy = new Mock<IDispatchStrategy>();
        mockStrategy
            .Setup(s => s.SelectElevator(
                It.IsAny<IEnumerable<IElevator>>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns((IElevator?)null);

        var controller = new ElevatorController([], mockStrategy.Object);

        controller.RequestElevator(5, 2);

        Assert.Equal(1, controller.PendingRequestCount);
    }

    [Fact]
    public void RequestElevator_DispatchesSecondElevator_WhenFirstCannotFitAllPassengers()
    {
        var firstElevator = new Mock<IElevator>();
        firstElevator.Setup(e => e.Capacity).Returns(10);
        firstElevator.Setup(e => e.PassengerCount).Returns(8);
        firstElevator.Setup(e => e.CanAcceptPassengers).Returns(true);
        firstElevator.Setup(e => e.State).Returns(ElevatorState.Idle);

        var secondElevator = new Mock<IElevator>();
        secondElevator.Setup(e => e.Capacity).Returns(10);
        secondElevator.Setup(e => e.PassengerCount).Returns(0);
        secondElevator.Setup(e => e.CanAcceptPassengers).Returns(true);
        secondElevator.Setup(e => e.State).Returns(ElevatorState.Idle);

        var mockStrategy = new Mock<IDispatchStrategy>();
        mockStrategy
            .SetupSequence(s => s.SelectElevator(
                It.IsAny<IEnumerable<IElevator>>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(firstElevator.Object)
            .Returns(secondElevator.Object);

        var controller = new ElevatorController(
            [firstElevator.Object, secondElevator.Object],
            mockStrategy.Object);

        controller.RequestElevator(5, 5);

        firstElevator.Verify(e => e.AddPassengers(2), Times.Once);
        secondElevator.Verify(e => e.AddPassengers(3), Times.Once);
    }
}