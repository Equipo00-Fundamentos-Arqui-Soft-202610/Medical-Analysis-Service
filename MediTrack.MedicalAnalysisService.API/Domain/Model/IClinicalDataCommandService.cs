using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IClinicalDataCommandService
{
    Task<ClinicalRecord> HandleAsync(RegisterClinicalRecordCommand command);
    Task<string> HandleAsync(ImportClinicalDatasetCommand command);
}
