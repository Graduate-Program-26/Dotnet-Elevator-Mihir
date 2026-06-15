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

        const int directionChangePenalty = 5;
        var penalty = CountDirectionChanges(elevator.CurrentFloor, startingFloor, destinations) * directionChangePenalty;

        return ((double)totalDistance / elevator.Speed) + penalty;
    }

    private static int CountDirectionChanges(int startFloor, int originFloor, IEnumerable<int> destinations)
    {
        var allStops = new[] { startFloor, originFloor }
            .Concat(destinations)
            .ToList();

        var changes = 0;
        for (int i = 1; i < allStops.Count - 1; i++)
        {
            var incoming = allStops[i] - allStops[i - 1];
            var outgoing = allStops[i + 1] - allStops[i];

            if (incoming != 0 && outgoing != 0 &&
                Math.Sign(incoming) != Math.Sign(outgoing))
            {
                changes++;
            }
        }
        return changes;
    }
}
