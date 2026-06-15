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
    public void RequestElevator_ThrowsInvalidFloorException_WhenFloorBelowMinimum()
    {
        var controller = new ElevatorController([], new Mock<IDispatchStrategy>().Object);

        Assert.Throws<InvalidFloorException>(
            () => controller.RequestElevator(0, [new Passenger(1, 5)]));
    }

    [Fact]
    public void RequestElevator_ThrowsInvalidFloorException_WhenFloorAboveMaximum()
    {
        var controller = new ElevatorController([], new Mock<IDispatchStrategy>().Object);

        Assert.Throws<InvalidFloorException>(
            () => controller.RequestElevator(21, [new Passenger(21, 5)]));
    }

    [Fact]
    public void RequestElevator_ThrowsArgumentOutOfRangeException_WhenPassengerListIsEmpty()
    {
        var controller = new ElevatorController([], new Mock<IDispatchStrategy>().Object);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => controller.RequestElevator(5, Enumerable.Empty<Passenger>()));
    }

    [Fact]
    public void RequestElevator_QueuesAllRemainingRequests_WhenNoElevatorAvailable()
    {
        var mockStrategy = new Mock<IDispatchStrategy>();
        mockStrategy
            .Setup(s => s.SelectElevator(It.IsAny<IEnumerable<IElevator>>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((IElevator?)null);

        var controller = new ElevatorController([], mockStrategy.Object);
        var passengers = new List<Passenger> { new(5, 10), new(5, 12) };

        controller.RequestElevator(5, passengers);

        Assert.Equal(2, controller.PendingRequestCount);
    }

    [Fact]
    public void RequestElevator_DispatchesToSecondElevator_WhenFirstReachesCapacity()
    {
        var firstElevator = new Mock<IElevator>();

        firstElevator.SetupSequence(e => e.CanAcceptPassengers)
            .Returns(true)
            .Returns(false);

        var secondElevator = new Mock<IElevator>();

        secondElevator.SetupSequence(e => e.CanAcceptPassengers)
            .Returns(true)
            .Returns(false);

        var mockStrategy = new Mock<IDispatchStrategy>();
        mockStrategy
            .SetupSequence(s => s.SelectElevator(It.IsAny<IEnumerable<IElevator>>(), 5, It.IsAny<int>()))
            .Returns(firstElevator.Object)
            .Returns(secondElevator.Object);

        var controller = new ElevatorController(
            [firstElevator.Object, secondElevator.Object],
            mockStrategy.Object);

        var p1 = new Passenger(5, 10);
        var p2 = new Passenger(5, 15);

        controller.RequestElevator(5, [p1, p2]);

        firstElevator.Verify(e => e.BoardPassenger(p1), Times.Once);
        firstElevator.Verify(e => e.MoveToFloor(10), Times.Once);
        firstElevator.Verify(e => e.DeboardPassengers(), Times.Once);

        secondElevator.Verify(e => e.BoardPassenger(p2), Times.Once);
        secondElevator.Verify(e => e.MoveToFloor(15), Times.Once);
        secondElevator.Verify(e => e.DeboardPassengers(), Times.Once);
    }

    [Fact]
    public void PassengerElevator_HasCorrectSpeed()
    {
        var elevator = new PassengerElevator();
        Assert.Equal(1, elevator.Speed);
    }

    [Fact]
    public void FreightElevator_HasLowerSpeed_ThanPassengerElevator()
    {
        var freight = new FreightElevator();
        var passenger = new PassengerElevator();
        Assert.True(freight.Speed < passenger.Speed);
    }

    [Fact]
    public void HighSpeedElevator_HasHigherSpeed_ThanPassengerElevator()
    {
        var highSpeed = new HighSpeedElevator();
        var passenger = new PassengerElevator();
        Assert.True(highSpeed.Speed > passenger.Speed);
    }

    [Fact]
    public void CalculateCost_ReturnsCorrectCost_ForSingleDestination()
    {
        var elevator = new PassengerElevator(startFloor: 1); // speed 1
        var passengers = new List<Passenger>
        {
            new Passenger(StartingFloor: 1, DestinationFloor: 5)
        };

        var cost = TripCostCalculator.Calculate(elevator, startingFloor: 1, passengers);

        Assert.Equal(4, cost);
    }

    [Fact]
    public void CalculateCost_IncludesDistanceToOrigin()
    {
        var elevator = new PassengerElevator(startFloor: 3);
        var passengers = new List<Passenger>
        {
            new Passenger(StartingFloor: 1, DestinationFloor: 5)
        };

        var cost = TripCostCalculator.Calculate(elevator, startingFloor: 1, passengers);

        Assert.Equal(6, cost);
    }

    [Fact]
    public void CalculateCost_AccountsForElevatorSpeed()
    {
        var elevator = new HighSpeedElevator(startFloor: 1);
        var passengers = new List<Passenger>
        {
            new Passenger(StartingFloor: 1, DestinationFloor: 7)
        };

        var cost = TripCostCalculator.Calculate(elevator, startingFloor: 1, passengers);

        Assert.Equal(2, cost);
    }

    [Fact]
    public void CalculateCost_SumsDistanceAcrossMultipleStops()
    {
        var elevator = new PassengerElevator(startFloor: 1);
        var passengers = new List<Passenger>
        {
            new Passenger(StartingFloor: 1, DestinationFloor: 3),
            new Passenger(StartingFloor: 1, DestinationFloor: 7),
            new Passenger(StartingFloor: 1, DestinationFloor: 5)
        };

        var cost = TripCostCalculator.Calculate(elevator, startingFloor: 1, passengers);

        Assert.Equal(6, cost);
    }

    [Fact]
    public void CalculateCost_LowerCost_ForFasterElevator()
    {
        var passenger = new PassengerElevator(startFloor: 1);
        var highSpeed = new HighSpeedElevator(startFloor: 1);

        var passengers = new List<Passenger>
        {
            new Passenger(StartingFloor: 1, DestinationFloor: 9)
        };

        var passengerCost = TripCostCalculator.Calculate(passenger, 1, passengers);
        var highSpeedCost = TripCostCalculator.Calculate(highSpeed, 1, passengers);

        Assert.True(highSpeedCost < passengerCost);
    }

    [Fact]
    public void CalculateCost_AddsPenalty_WhenElevatorMustReverseDirection()
    {
        var elevator = new PassengerElevator(startFloor: 5);
        elevator.MoveToFloor(8);

        var passengers = new List<Passenger>
        {
            new Passenger(StartingFloor: 5, DestinationFloor: 2)
        };

        var costWithReversal = TripCostCalculator.Calculate(elevator, startingFloor: 5, passengers);
        var elevatorGoingSameWay = new PassengerElevator(startFloor: 1);
        var costSameDirection = TripCostCalculator.Calculate(elevatorGoingSameWay, startingFloor: 5, passengers);

        Assert.True(costWithReversal > costSameDirection);
    }

    [Fact]
    public void SelectElevator_PrefersFasterElevator_WhenEquidistant()
    {
        var passenger = new PassengerElevator(startFloor: 1);
        var highSpeed = new HighSpeedElevator(startFloor: 1);
        var strategy = new NearestAvailableDispatchStrategy();

        var passengers = new List<Passenger>
        {
            new Passenger(StartingFloor: 1, DestinationFloor: 9)
        };

        var selected = strategy.SelectElevator([passenger, highSpeed], 1, passengers);

        Assert.Equal(highSpeed, selected);
    }

    [Fact]
    public void SelectElevator_PrefersElevatorGoingSameDirection()
    {
        var goingUp = new PassengerElevator(startFloor: 1);
        goingUp.MoveToFloor(3);

        var goingDown = new PassengerElevator(startFloor: 10);
        goingDown.MoveToFloor(7);

        var strategy = new NearestAvailableDispatchStrategy();

        var passengers = new List<Passenger>
        {
            new Passenger(StartingFloor: 5, DestinationFloor: 8)
        };

        var selected = strategy.SelectElevator([goingUp, goingDown], 5, passengers);

        Assert.Equal(goingUp, selected);
    }
}
