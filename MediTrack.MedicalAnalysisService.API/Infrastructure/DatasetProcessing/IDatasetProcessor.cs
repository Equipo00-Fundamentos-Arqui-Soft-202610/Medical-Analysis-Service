using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.DatasetProcessing;

public interface IDatasetProcessor
{
    Task<IEnumerable<ClinicalRecord>> ProcessAsync(Stream dataStream, string importBatchId);
}
