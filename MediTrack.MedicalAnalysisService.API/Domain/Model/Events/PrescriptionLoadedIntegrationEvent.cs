namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Events;

public record PrescriptionLoadedIntegrationEvent(
    int PatientId,
    int PrescriptionId,
    DateTime OccurredAtUtc);
