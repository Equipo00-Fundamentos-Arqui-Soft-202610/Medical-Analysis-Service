namespace MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

public class ComplianceCategory
{
    public static readonly ComplianceCategory Medication = new("medication");
    public static readonly ComplianceCategory Appointment = new("appointment");

    private static readonly Dictionary<string, ComplianceCategory> ValidValues = new()
    {
        { "medication", Medication },
        { "appointment", Appointment }
    };

    public string Value { get; }

    private ComplianceCategory(string value)
    {
        Value = value;
    }

    public static ComplianceCategory From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("ComplianceCategory cannot be empty", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();
        if (!ValidValues.TryGetValue(normalized, out var category))
            throw new ArgumentException("ComplianceCategory must be medication or appointment", nameof(value));

        return category;
    }

    public override bool Equals(object? obj) =>
        obj is ComplianceCategory other && other.Value == Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(ComplianceCategory c) => c.Value;
    public static implicit operator ComplianceCategory(string v) => From(v);
}
