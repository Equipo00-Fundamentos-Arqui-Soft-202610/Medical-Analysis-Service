using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;

public class AdherenceMetric
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public ComplianceCategory Category { get; set; } = null!;
    public int TotalScheduled { get; set; }
    public int TotalCompliant { get; set; }
    public int TotalMissed { get; set; }
    public AdherenceRate Rate { get; set; } = AdherenceRate.From(0);
    public DateTime LastUpdatedAt { get; set; }

    public AdherenceMetric() { }

    public AdherenceMetric(int patientId, string category)
    {
        if (patientId <= 0)
            throw new ArgumentException("PatientId must be greater than 0", nameof(patientId));

        PatientId = patientId;
        Category = ComplianceCategory.From(category);
        TotalScheduled = 0;
        TotalCompliant = 0;
        TotalMissed = 0;
        Rate = AdherenceRate.From(0);
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void Recalculate(int totalScheduled, int totalCompliant, int totalMissed)
    {
        if (totalScheduled < 0)
            throw new ArgumentException("TotalScheduled cannot be negative", nameof(totalScheduled));
        if (totalCompliant < 0)
            throw new ArgumentException("TotalCompliant cannot be negative", nameof(totalCompliant));
        if (totalMissed < 0)
            throw new ArgumentException("TotalMissed cannot be negative", nameof(totalMissed));

        TotalScheduled = totalScheduled;
        TotalCompliant = totalCompliant;
        TotalMissed = totalMissed;
        Rate = AdherenceRate.Calculate(totalScheduled, totalCompliant);
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void RegisterEvent(bool compliant)
    {
        TotalScheduled++;
        if (compliant)
            TotalCompliant++;
        else
            TotalMissed++;

        Rate = AdherenceRate.Calculate(TotalScheduled, TotalCompliant);
        LastUpdatedAt = DateTime.UtcNow;
    }
}
