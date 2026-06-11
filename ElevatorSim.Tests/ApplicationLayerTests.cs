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
}