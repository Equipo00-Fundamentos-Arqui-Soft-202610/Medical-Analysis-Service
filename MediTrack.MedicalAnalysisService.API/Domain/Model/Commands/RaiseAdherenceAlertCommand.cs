namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;

public record RaiseAdherenceAlertCommand(
    int PatientId,
    string Severity,
    string Reason);
