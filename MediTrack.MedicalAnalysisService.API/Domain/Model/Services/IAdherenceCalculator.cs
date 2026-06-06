using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Services;

public interface IAdherenceCalculator
{
    ComplianceCategory Category { get; }
    AdherenceRate Calculate(int totalScheduled, int totalCompliant);
    bool ShouldRaiseAlert(AdherenceRate rate);
    string DetermineAlertSeverity(AdherenceRate rate);
}
