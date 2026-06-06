namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

public record RaiseAlertResource(
    int PatientId,
    string Severity,
    string Reason);

public record AlertResource(
    int Id,
    int PatientId,
    string Severity,
    string Status,
    string Reason,
    DateTime TriggeredAt,
    DateTime? AcknowledgedAt);
