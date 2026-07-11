namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;

public record ImportClinicalDatasetCommand(
    Stream DataStream,
    string ImportBatchId,
    int PatientId,
    DateTime RecordDate,
    string Diagnosis,
    string? Notes);
