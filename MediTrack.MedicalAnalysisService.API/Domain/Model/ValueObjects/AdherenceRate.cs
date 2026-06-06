namespace MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

public class AdherenceRate
{
    public decimal Value { get; }

    private AdherenceRate(decimal value)
    {
        Value = value;
    }

    public static AdherenceRate From(decimal value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentException("AdherenceRate must be between 0 and 100", nameof(value));

        return new AdherenceRate(Math.Round(value, 2));
    }

    public static AdherenceRate Calculate(int totalScheduled, int totalCompliant)
    {
        if (totalScheduled <= 0)
            return new AdherenceRate(0);

        var rate = (decimal)totalCompliant / totalScheduled * 100;
        return From(Math.Min(rate, 100));
    }

    public override bool Equals(object? obj) =>
        obj is AdherenceRate other && other.Value == Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => $"{Value:F2}%";

    public static implicit operator decimal(AdherenceRate rate) => rate.Value;
    public static implicit operator AdherenceRate(decimal value) => From(value);
}
