namespace MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

public class AlertStatus
{
    public static readonly AlertStatus Open = new("open");
    public static readonly AlertStatus Acknowledged = new("acknowledged");
    public static readonly AlertStatus Resolved = new("resolved");

    private static readonly Dictionary<string, AlertStatus> ValidValues = new()
    {
        { "open", Open },
        { "acknowledged", Acknowledged },
        { "resolved", Resolved }
    };

    public string Value { get; }

    private AlertStatus(string value)
    {
        Value = value;
    }

    public static AlertStatus From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("AlertStatus cannot be empty", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();
        if (!ValidValues.TryGetValue(normalized, out var status))
            throw new ArgumentException("AlertStatus must be open, acknowledged, or resolved", nameof(value));

        return status;
    }

    public bool IsOpen => Value == "open";

    public override bool Equals(object? obj) =>
        obj is AlertStatus other && other.Value == Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(AlertStatus s) => s.Value;
    public static implicit operator AlertStatus(string v) => From(v);
}
