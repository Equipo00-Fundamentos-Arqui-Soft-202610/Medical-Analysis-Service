using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC;

public class AdherenceMetricRepository : IAdherenceMetricRepository
{
    private readonly MedicalAnalysisDbContext _context;

    public AdherenceMetricRepository(MedicalAnalysisDbContext context)
    {
        _context = context;
    }

    public async Task<AdherenceMetric> AddAsync(AdherenceMetric metric)
    {
        _context.AdherenceMetrics.Add(metric);
        await _context.SaveChangesAsync();
        return metric;
    }

    public async Task<AdherenceMetric> UpdateAsync(AdherenceMetric metric)
    {
        _context.AdherenceMetrics.Update(metric);
        await _context.SaveChangesAsync();
        return metric;
    }

    public async Task<AdherenceMetric?> FindByPatientIdAndCategoryAsync(int patientId, string category)
    {
        return await _context.AdherenceMetrics
            .FirstOrDefaultAsync(m => m.PatientId == patientId && m.Category.Value == category);
    }

    public async Task<IEnumerable<AdherenceMetric>> FindByPatientIdAsync(int patientId)
    {
        return await _context.AdherenceMetrics
            .Where(m => m.PatientId == patientId)
            .ToListAsync();
    }
}
