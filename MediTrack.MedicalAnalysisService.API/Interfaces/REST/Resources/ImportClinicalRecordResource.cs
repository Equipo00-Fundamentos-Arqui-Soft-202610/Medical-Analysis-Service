namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

public record ImportClinicalRecordResource(
    int PatientId,
    DateTime RecordDate,
    string Diagnosis,
    string? Notes);
