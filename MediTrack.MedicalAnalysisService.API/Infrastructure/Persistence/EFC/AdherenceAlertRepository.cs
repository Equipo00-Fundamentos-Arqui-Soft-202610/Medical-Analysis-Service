using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;
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

        // Comparar el value object completo (no ".Value") -- EF Core no puede
        // traducir el acceso a una propiedad anidada de un tipo value-converted,
        // esto revienta con "could not be translated" en tiempo de ejecución.
        var statusVO = AlertStatus.From(status);
        return await _context.AdherenceAlerts
            .Where(a => a.Status == statusVO)
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
