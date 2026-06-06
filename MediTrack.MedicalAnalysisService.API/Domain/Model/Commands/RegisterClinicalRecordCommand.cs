namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;

public record RegisterClinicalRecordCommand(
    int PatientId,
    DateTime RecordDate,
    string Diagnosis,
    string? Notes);
