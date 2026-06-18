namespace ElevatorSim.Infrastructure.Logging;

public class FileLogViewer(string logFilePath) : ILogViewer
{
    private readonly string _logFilePath = logFilePath;

    public IReadOnlyList<string> GetRecentEntries(int count)
    {
        if (!File.Exists(_logFilePath))
        {
            return [];
        }

        using var stream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);

        var lines = new List<string>();
        string? line;
        while ((line = reader.ReadLine()) is not null)
            lines.Add(line);

        return lines
            .TakeLast(count)
            .ToList();
    }
}
