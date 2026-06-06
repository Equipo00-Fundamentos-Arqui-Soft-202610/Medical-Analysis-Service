using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.QueryServices;

public class ClinicalDataQueryService : IClinicalDataQueryService
{
    private readonly IClinicalRecordRepository _recordRepository;

    public ClinicalDataQueryService(IClinicalRecordRepository recordRepository)
    {
        _recordRepository = recordRepository;
    }

    public async Task<IEnumerable<ClinicalRecord>> HandleAsync(GetClinicalHistoryByPatientIdQuery query)
    {
        if (query.PatientId <= 0)
            throw new ArgumentException("PatientId must be greater than 0", nameof(query.PatientId));

        return await _recordRepository.FindByPatientIdAsync(query.PatientId);
    }
}
