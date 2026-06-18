using ElevatorSim.Domain.Models;

namespace ElevatorSim.Application.Utilities;

/// <summary>
/// A utility class that sorts passenger destination floors by proximity to a given starting floor.
/// </summary>
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
