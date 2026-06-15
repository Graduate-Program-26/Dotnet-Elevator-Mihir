public static class DestinationSorter
{
    public static IEnumerable<int> SortByProximity(int startingFloor, IEnumerable<Passenger> passengers)
    {
        return passengers
            .Select(passenger => passenger.DestinationFloor)
            .Distinct()
            .OrderBy(floor => Math.Abs(floor - startingFloor));
    }
}