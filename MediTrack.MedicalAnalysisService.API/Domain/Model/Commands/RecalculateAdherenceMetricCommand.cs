namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;

public record RecalculateAdherenceMetricCommand(
    int PatientId,
    string Category,
    bool WasCompliant);
