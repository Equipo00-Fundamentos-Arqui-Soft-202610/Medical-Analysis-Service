using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC;

public class ClinicalRecordRepository : IClinicalRecordRepository
{
    private readonly MedicalAnalysisDbContext _context;

    public ClinicalRecordRepository(MedicalAnalysisDbContext context)
    {
        _context = context;
    }

    public async Task<ClinicalRecord> AddAsync(ClinicalRecord record)
    {
        _context.ClinicalRecords.Add(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<IEnumerable<ClinicalRecord>> FindByPatientIdAsync(int patientId)
    {
        return await _context.ClinicalRecords
            .Where(r => r.PatientId == patientId)
            .OrderByDescending(r => r.RecordDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClinicalRecord>> FindByPatientIdAndDateRangeAsync(int patientId, DateTime from, DateTime to)
    {
        return await _context.ClinicalRecords
            .Where(r => r.PatientId == patientId && r.RecordDate >= from && r.RecordDate <= to)
            .OrderByDescending(r => r.RecordDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClinicalRecord>> FindByImportBatchIdAsync(string importBatchId)
    {
        return await _context.ClinicalRecords
            .Where(r => r.ImportBatchId == importBatchId)
            .ToListAsync();
    }
}
