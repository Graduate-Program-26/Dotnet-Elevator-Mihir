public static class PassengerDistributor
{
    public static Dictionary<IElevator, List<Passenger>> Distribute(IEnumerable<Passenger> passengers, IReadOnlyList<IElevator> elevators, int originFloor)
    {
        var assignments = elevators
            .ToDictionary(elevator => elevator, _ => new List<Passenger>());

        var passengerList = passengers.ToList();

        if (elevators.Count == 0)
        {
            return assignments;
        }

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
                .Where(elevator => elevator.CanAcceptPassengers)
                .OrderBy(elevator => TripCostCalculator.Calculate(elevator, originFloor, group))
                .FirstOrDefault();

            best ??= elevators
                .Where(elevator => elevator.CanAcceptPassengers)
                .OrderBy(elevator => TripCostCalculator.Calculate(elevator, originFloor, group))
                .FirstOrDefault();

            if (best is null) continue;

            assignments[best].AddRange(group);
            assignedElevators.Add(best);
        }
        return assignments;
    }
}
