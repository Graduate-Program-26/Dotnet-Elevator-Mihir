public static class PassengerGrouper
{
    public static List<List<Passenger>> Group(
        IEnumerable<Passenger> passengers,
        int groupCount)
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