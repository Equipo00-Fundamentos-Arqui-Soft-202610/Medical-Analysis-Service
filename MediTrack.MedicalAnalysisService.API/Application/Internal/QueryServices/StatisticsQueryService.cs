using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.QueryServices;

public class StatisticsQueryService : IStatisticsQueryService
{
    private readonly IComplianceStatisticRepository _statisticRepository;

    public StatisticsQueryService(IComplianceStatisticRepository statisticRepository)
    {
        _statisticRepository = statisticRepository;
    }

    public async Task<IEnumerable<ComplianceStatistic>> HandleAsync(GetComplianceStatisticsQuery query)
    {
        return await _statisticRepository.FindByCategoryAndRangeAsync(query.Category, query.From, query.To);
    }

    public async Task<IEnumerable<ComplianceStatistic>> HandleAsync(GetAppointmentStatisticsQuery query)
    {
        return await _statisticRepository.FindByCategoryAndRangeAsync("appointment", query.From, query.To);
    }
}
