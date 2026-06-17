using System.Runtime.CompilerServices;

using Moq;

namespace ElevatorSim.Tests;

public class PassengerTests
{
    [Fact]
    public void Passenger_HasStartingAndDestinationFloor()
    {
        var passenger = new Passenger(StartingFloor: 1, DestinationFloor: 5);

        Assert.Equal(1, passenger.StartingFloor);
        Assert.Equal(5, passenger.DestinationFloor);
    }

    [Fact]
    public void BoardPassenger_AddsPassengerWithDestination()
    {
        var elevator = new PassengerElevator(capacity: 10, startFloor: 1);
        var passenger = new Passenger(StartingFloor: 1, DestinationFloor: 5);

        elevator.BoardPassenger(passenger);

        Assert.Equal(1, elevator.PassengerCount);
    }

    [Fact]
    public void BoardPassenger_TracksPassengerDestination()
    {

        var elevator = new PassengerElevator(capacity: 10, startFloor: 1);
        var passenger = new Passenger(StartingFloor: 1, DestinationFloor: 5);

        elevator.BoardPassenger(passenger);

        Assert.Contains(5, elevator.DestinationFloors);
    }

    [Fact]
    public void DeboardPassengers_RemovesPassengerAtCurrentFloor()
    {
        var elevator = new PassengerElevator(capacity: 10, startFloor: 1);
        elevator.BoardPassenger(new Passenger(StartingFloor: 1, DestinationFloor: 5));
        elevator.BoardPassenger(new Passenger(StartingFloor: 1, DestinationFloor: 3));

        elevator.MoveToFloor(5);
        elevator.DeboardPassengers();

        Assert.Equal(1, elevator.PassengerCount);
    }

    [Fact]
    public void DeboardPassengers_RemovesOnlyPassengersAtCurrentFloor()
    {
        var elevator = new PassengerElevator(capacity: 10, startFloor: 1);
        elevator.BoardPassenger(new Passenger(StartingFloor: 1, DestinationFloor: 5));
        elevator.BoardPassenger(new Passenger(StartingFloor: 1, DestinationFloor: 5));
        elevator.BoardPassenger(new Passenger(StartingFloor: 1, DestinationFloor: 3));

        elevator.MoveToFloor(5);
        elevator.DeboardPassengers();

        Assert.Equal(1, elevator.PassengerCount);
    }

    [Fact]
    public void DeboardPassengers_DoesNothing_WhenNoPassengerAtCurrentFloor()
    {
        var elevator = new PassengerElevator(capacity: 10, startFloor: 1);
        elevator.BoardPassenger(new Passenger(StartingFloor: 1, DestinationFloor: 5));

        elevator.MoveToFloor(3);
        elevator.DeboardPassengers();

        Assert.Equal(1, elevator.PassengerCount);
    }

    [Fact]
    public void RequestElevator_PassengerDeboardsAtDestination()
    {
        var elevator = new PassengerElevator(capacity: 10, startFloor: 1);
        var mockStrategy = new Mock<IDispatchStrategy>();
        mockStrategy
            .Setup(s => s.SelectElevator(
                It.IsAny<IEnumerable<IElevator>>(),
                It.IsAny<int>(),
                It.IsAny<IEnumerable<Passenger>>()))
            .Returns(elevator);

        var controller = new ElevatorController([elevator], mockStrategy.Object);

        var passengers = new List<Passenger>
        {
            new Passenger(StartingFloor: 1, DestinationFloor: 5),
            new Passenger(StartingFloor: 1, DestinationFloor: 5)
        };

        controller.RequestElevator(1, passengers);

        Assert.Equal(0, elevator.PassengerCount);
        Assert.Equal(5, elevator.CurrentFloor);
    }
}
