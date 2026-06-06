using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IClinicalDataQueryService
{
    Task<IEnumerable<ClinicalRecord>> HandleAsync(GetClinicalHistoryByPatientIdQuery query);
}
