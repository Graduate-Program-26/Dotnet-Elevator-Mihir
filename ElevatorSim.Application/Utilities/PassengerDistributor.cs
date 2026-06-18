using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Domain.Models;

namespace ElevatorSim.Application.Utilities;

/// <summary>
/// A utility class that distributes passengers to elevators based on destination floor proximity and trip cost calculations.
/// </summary>
public static class PassengerDistributor
{
    /// <summary>
    /// Groups passengers by destination floor proximity and assigns them to elevators based on trip cost calculations.
    /// The capacity of elevators is respected and passengers are distributed to minimize total travel time and direction changes.
    /// </summary>
    /// <param name="passengers">The list of passengers to distribute.</param>
    /// <param name="elevators">The list of available elevators.</param>
    /// <param name="startingFloor">The floor from which passengers are requesting an elevator.</param>
    /// <returns>A dictionary mapping each elevator to the list of passengers assigneed to it.</returns>
    public static Dictionary<IElevator, List<Passenger>> Distribute(IEnumerable<Passenger> passengers, IReadOnlyList<IElevator> elevators, int startingFloor)
    {
        var assignments = elevators
            .ToDictionary(elevator => elevator, _ => new List<Passenger>());

        var assignedCounts = elevators
            .ToDictionary(e => e, _ => 0);

        var passengerList = passengers.ToList();

        if (elevators.Count == 0) return assignments;

        if (elevators.Count == 1)
        {
            assignments[elevators[0]].AddRange(passengerList);
            return assignments;
        }

        var groups = PassengerGrouper.Group(passengerList, elevators.Count);
        var assignedElevators = new HashSet<IElevator>();

        foreach (var group in groups.OrderByDescending(g => g.Count))
        {
            var best = elevators
                .Where(elevator => !assignedElevators.Contains(elevator))
                .Where(elevator => HasRemainingCapacity(elevator, assignedCounts))
                .OrderBy(elevator => TripCostCalculator.Calculate(
                    elevator, startingFloor, group))
                .FirstOrDefault();

            best ??= elevators
                .Where(elevator => HasRemainingCapacity(elevator, assignedCounts))
                .OrderBy(elevator => TripCostCalculator.Calculate(
                    elevator, startingFloor, group))
                .FirstOrDefault();

            if (best is null) continue;

            assignments[best].AddRange(group);
            assignedCounts[best] += group.Count;
            assignedElevators.Add(best);
        }

        return assignments;
    }

    private static bool HasRemainingCapacity(
        IElevator elevator,
        Dictionary<IElevator, int> assignedCounts)
    {
        var totalAssigned = assignedCounts[elevator];
        var available = elevator.Capacity - elevator.PassengerCount;
        return available - totalAssigned > 0;
    }
}
