namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Events;

public record ComplianceRegisteredIntegrationEvent(
    int PatientId,
    string Category,
    bool WasCompliant,
    DateTime OccurredAt);
