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
}