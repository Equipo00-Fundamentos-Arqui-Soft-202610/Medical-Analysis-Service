namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;

public record ImportClinicalDatasetCommand(Stream DataStream, string ImportBatchId);
