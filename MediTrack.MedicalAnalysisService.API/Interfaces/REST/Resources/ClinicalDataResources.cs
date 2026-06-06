namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;

public record CreateClinicalRecordResource(
    int PatientId,
    DateTime RecordDate,
    string Diagnosis,
    string? Notes);

public record ImportClinicalDatasetResource(string ImportBatchId);

public record ClinicalRecordResource(
    int Id,
    int PatientId,
    DateTime RecordDate,
    string Diagnosis,
    string? Notes,
    string Source,
    string? ImportBatchId,
    DateTime CreatedAt);
