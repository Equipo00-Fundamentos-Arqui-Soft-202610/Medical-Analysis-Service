using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Services;

public class MedicationAdherenceStrategy : IAdherenceCalculator
{
    private const decimal AlertThreshold = 70m;
    private const decimal CriticalThreshold = 50m;

    public ComplianceCategory Category => ComplianceCategory.Medication;

    public AdherenceRate Calculate(int totalScheduled, int totalCompliant) =>
        AdherenceRate.Calculate(totalScheduled, totalCompliant);

    public bool ShouldRaiseAlert(AdherenceRate rate) =>
        rate.Value < AlertThreshold;

    public string DetermineAlertSeverity(AdherenceRate rate) =>
        rate.Value < CriticalThreshold ? "critical" : "warning";
}
