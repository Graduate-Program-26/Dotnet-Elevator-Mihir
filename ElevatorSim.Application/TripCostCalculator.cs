public static class TripCostCalculator
{
    public static double Calculate(IElevator elevator, int startingFloor, IEnumerable<Passenger> passengers)
    {
        var passengerList = passengers.ToList();

        if (passengerList.Count == 0)
        {
            return 0;
        }

        var distanceToOrigin = Math.Abs(elevator.CurrentFloor - startingFloor);

        var destinations = DestinationSorter
            .SortByProximity(startingFloor, passengerList)
            .ToList();

        var totalDistance = distanceToOrigin;
        var currentFloor = startingFloor;

        foreach (var destination in destinations)
        {
            totalDistance += Math.Abs(destination - currentFloor);
            currentFloor = destination;
        }

        return (double)totalDistance / elevator.Speed;
    }
}