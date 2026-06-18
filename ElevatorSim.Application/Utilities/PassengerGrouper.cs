using ElevatorSim.Domain.Models;

namespace ElevatorSim.Application.Utilities;

/// <summary>
/// A utilit class that groups passengers by destination floor proximity to optimize elevator assignments and minimize total trip cost.
/// </summary>
public static class PassengerGrouper
{
    /// <summary>
    /// Groups passengers by destination floor proximity to optimize elevator assignments and minimize total trip cost.
    /// </summary>
    /// <param name="passengers">The list of passengers to group.</param>
    /// <param name="groupCount">The number of groups to create</param>
    /// <returns>A list of groups of passengers.</returns>
    public static List<List<Passenger>> Group(IEnumerable<Passenger> passengers, int groupCount)
    {
        var sorted = passengers
            .OrderBy(p => p.DestinationFloor)
            .ToList();

        if (sorted.Count == 0) return [];
        if (groupCount <= 1 || sorted.Count <= groupCount)
            return [sorted];

        var gaps = sorted
            .Zip(sorted.Skip(1), (a, b) => new
            {
                Index = sorted.IndexOf(b),
                Gap = b.DestinationFloor - a.DestinationFloor
            })
            .OrderByDescending(g => g.Gap)
            .Take(groupCount - 1)
            .Select(g => g.Index)
            .OrderBy(i => i)
            .ToList();

        var groups = new List<List<Passenger>>();
        var start = 0;

        foreach (var splitIndex in gaps)
        {
            groups.Add(sorted.GetRange(start, splitIndex - start));
            start = splitIndex;
        }

        groups.Add(sorted.GetRange(start, sorted.Count - start));

        return groups.Where(g => g.Count > 0).ToList();
    }
}
