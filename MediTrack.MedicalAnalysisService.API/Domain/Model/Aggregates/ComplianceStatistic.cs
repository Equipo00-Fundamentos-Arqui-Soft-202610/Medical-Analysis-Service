using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;

public class ComplianceStatistic
{
    public int Id { get; set; }
    public ComplianceCategory Category { get; set; } = null!;
    public DateTime WindowStart { get; set; }
    public DateTime WindowEnd { get; set; }
    public int TotalScheduled { get; set; }
    public int TotalCompliant { get; set; }
    public int TotalMissed { get; set; }
    public AdherenceRate ComplianceRate { get; set; } = AdherenceRate.From(0);
    public DateTime CalculatedAt { get; set; }

    public ComplianceStatistic() { }

    public ComplianceStatistic(string category, DateTime windowStart, DateTime windowEnd)
    {
        if (windowStart >= windowEnd)
            throw new ArgumentException("WindowStart must be before WindowEnd");

        Category = ComplianceCategory.From(category);
        WindowStart = windowStart;
        WindowEnd = windowEnd;
        TotalScheduled = 0;
        TotalCompliant = 0;
        TotalMissed = 0;
        ComplianceRate = AdherenceRate.From(0);
        CalculatedAt = DateTime.UtcNow;
    }

    public void Update(int totalScheduled, int totalCompliant, int totalMissed)
    {
        TotalScheduled = totalScheduled;
        TotalCompliant = totalCompliant;
        TotalMissed = totalMissed;
        ComplianceRate = AdherenceRate.Calculate(totalScheduled, totalCompliant);
        CalculatedAt = DateTime.UtcNow;
    }
}
