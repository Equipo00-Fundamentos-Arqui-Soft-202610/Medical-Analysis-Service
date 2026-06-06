using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model;

public interface IAdherenceAlertRepository
{
    Task<AdherenceAlert> AddAsync(AdherenceAlert alert);
    Task<AdherenceAlert> UpdateAsync(AdherenceAlert alert);
    Task<AdherenceAlert?> FindByIdAsync(int id);
    Task<IEnumerable<AdherenceAlert>> FindByStatusAsync(string? status);
    Task<IEnumerable<AdherenceAlert>> FindByPatientIdAsync(int patientId);
}
