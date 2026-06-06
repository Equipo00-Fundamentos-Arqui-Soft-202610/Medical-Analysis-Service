namespace MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

public class AlertSeverity
{
    public static readonly AlertSeverity Info = new("info");
    public static readonly AlertSeverity Warning = new("warning");
    public static readonly AlertSeverity Critical = new("critical");

    private static readonly Dictionary<string, AlertSeverity> ValidValues = new()
    {
        { "info", Info },
        { "warning", Warning },
        { "critical", Critical }
    };

    public string Value { get; }

    private AlertSeverity(string value)
    {
        Value = value;
    }

    public static AlertSeverity From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("AlertSeverity cannot be empty", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();
        if (!ValidValues.TryGetValue(normalized, out var severity))
            throw new ArgumentException("AlertSeverity must be info, warning, or critical", nameof(value));

        return severity;
    }

    public override bool Equals(object? obj) =>
        obj is AlertSeverity other && other.Value == Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(AlertSeverity s) => s.Value;
    public static implicit operator AlertSeverity(string v) => From(v);
}
