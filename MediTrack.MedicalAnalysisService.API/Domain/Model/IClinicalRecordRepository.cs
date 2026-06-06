using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IClinicalRecordRepository
{
    Task<ClinicalRecord> AddAsync(ClinicalRecord record);
    Task<IEnumerable<ClinicalRecord>> FindByPatientIdAsync(int patientId);
    Task<IEnumerable<ClinicalRecord>> FindByImportBatchIdAsync(string importBatchId);
}
