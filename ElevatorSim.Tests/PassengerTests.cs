namespace ElevatorSim.Tests;

public class PassengerTests
{
    [Fact]
    public void Passenger_HasStartingAndDestinationFloor()
    {
        var passenger = new Passenger(startingFloor: 1, destinationFloor: 5);
        Assert.Equal(1, passenger.StartingFloor);
        Assert.Equal(5, passenger.DestinationFloor);
    }
}