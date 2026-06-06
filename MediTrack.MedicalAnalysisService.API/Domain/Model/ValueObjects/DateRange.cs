namespace MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

public class DateRange
{
    public DateTime WindowStart { get; }
    public DateTime WindowEnd { get; }

    private DateRange(DateTime windowStart, DateTime windowEnd)
    {
        WindowStart = windowStart;
        WindowEnd = windowEnd;
    }

    public static DateRange From(DateTime windowStart, DateTime windowEnd)
    {
        if (windowStart >= windowEnd)
            throw new ArgumentException("WindowStart must be before WindowEnd");

        return new DateRange(windowStart, windowEnd);
    }

    public bool Contains(DateTime date) =>
        date >= WindowStart && date <= WindowEnd;

    public override bool Equals(object? obj) =>
        obj is DateRange other && other.WindowStart == WindowStart && other.WindowEnd == WindowEnd;

    public override int GetHashCode() =>
        HashCode.Combine(WindowStart, WindowEnd);

    public override string ToString() =>
        $"{WindowStart:yyyy-MM-dd} — {WindowEnd:yyyy-MM-dd}";
}
