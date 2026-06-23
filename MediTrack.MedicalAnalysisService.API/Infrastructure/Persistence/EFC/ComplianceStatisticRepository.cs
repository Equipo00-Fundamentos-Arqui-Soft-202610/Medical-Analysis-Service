using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;
using MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC;

public class ComplianceStatisticRepository : IComplianceStatisticRepository
{
    private readonly MedicalAnalysisDbContext _context;

    public ComplianceStatisticRepository(MedicalAnalysisDbContext context)
    {
        _context = context;
    }

    public async Task<ComplianceStatistic> AddAsync(ComplianceStatistic statistic)
    {
        _context.ComplianceStatistics.Add(statistic);
        await _context.SaveChangesAsync();
        return statistic;
    }

    public async Task<ComplianceStatistic> UpdateAsync(ComplianceStatistic statistic)
    {
        _context.ComplianceStatistics.Update(statistic);
        await _context.SaveChangesAsync();
        return statistic;
    }

    public async Task<IEnumerable<ComplianceStatistic>> FindByCategoryAndRangeAsync(
        string category, DateTime from, DateTime to)
    {
        var categoryVO = ComplianceCategory.From(category);
        return await _context.ComplianceStatistics
            .Where(s => s.Category == categoryVO && s.WindowStart >= from && s.WindowEnd <= to)
            .OrderBy(s => s.WindowStart)
            .ToListAsync();
    }

    public async Task<IEnumerable<ComplianceStatistic>> FindByRangeAsync(DateTime from, DateTime to)
    {
        return await _context.ComplianceStatistics
            .Where(s => s.WindowStart >= from && s.WindowEnd <= to)
            .OrderBy(s => s.WindowStart)
            .ToListAsync();
    }
}
