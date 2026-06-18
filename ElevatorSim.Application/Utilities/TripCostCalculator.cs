using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Domain.Models;

namespace ElevatorSim.Application.Utilities;

/// <summary>
/// A utility class that calculated the estimated trip cost for an elevator.
/// </summary>
public static class TripCostCalculator
{
    /// <summary>
    /// Calculates the estimated trip cost for an elevator to pick up a group of passengers from a starting floor and deliver them to their destination floors.
    /// </summary>
    /// <param name="elevator">The elevator for which to calculate the trip cost.</param>
    /// <param name="startingFloor">The floor from which passengers are requesting an elevator.</param>
    /// <param name="passengers">The list of passengers to assign to the elevator</param>
    /// <returns>The estimated trip cost.</returns>
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
        var penalty = CountDirectionChanges(elevator, startingFloor, destinations) * directionChangePenalty;

        return ((double)totalDistance / elevator.Speed) + penalty;
    }

    private static int CountDirectionChanges(IElevator elevator, int startingFloor, IEnumerable<int> destinations)
    {
        if (elevator.Direction == ElevatorDirection.Stationary)
        {
            return 0;
        }

        var currentDirection = elevator.Direction == ElevatorDirection.Up ? 1 : -1;
        var changes = 0;
        var currentFloor = startingFloor;

        foreach (var destination in destinations)
        {
            if (destination == currentFloor) continue;

            var nextDirection = destination > currentFloor ? 1 : -1;

            if (nextDirection != currentDirection)
            {
                changes++;
                currentDirection = nextDirection;
            }

            currentFloor = destination;
        }

        if (startingFloor != elevator.CurrentFloor)
        {
            var directionToOrigin = startingFloor > elevator.CurrentFloor ? 1 : -1;
            if (directionToOrigin != currentDirection)
                changes++;
        }

        return changes;
    }
}
