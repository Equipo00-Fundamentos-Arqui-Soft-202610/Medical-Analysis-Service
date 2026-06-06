using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC;

public class AdherenceAlertRepository : IAdherenceAlertRepository
{
    private readonly MedicalAnalysisDbContext _context;

    public AdherenceAlertRepository(MedicalAnalysisDbContext context)
    {
        _context = context;
    }

    public async Task<AdherenceAlert> AddAsync(AdherenceAlert alert)
    {
        _context.AdherenceAlerts.Add(alert);
        await _context.SaveChangesAsync();
        return alert;
    }

    public async Task<AdherenceAlert> UpdateAsync(AdherenceAlert alert)
    {
        _context.AdherenceAlerts.Update(alert);
        await _context.SaveChangesAsync();
        return alert;
    }

    public async Task<AdherenceAlert?> FindByIdAsync(int id)
    {
        return await _context.AdherenceAlerts.FindAsync(id);
    }

    public async Task<IEnumerable<AdherenceAlert>> FindByStatusAsync(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return await _context.AdherenceAlerts
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();

        return await _context.AdherenceAlerts
            .Where(a => a.Status.Value == status.ToLowerInvariant())
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AdherenceAlert>> FindByPatientIdAsync(int patientId)
    {
        return await _context.AdherenceAlerts
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync();
    }
}
