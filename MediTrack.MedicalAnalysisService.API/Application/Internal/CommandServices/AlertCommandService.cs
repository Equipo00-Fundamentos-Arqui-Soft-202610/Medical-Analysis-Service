using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.CommandServices;

public class AlertCommandService : IAlertCommandService
{
    private readonly IAdherenceAlertRepository _alertRepository;

    public AlertCommandService(IAdherenceAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<AdherenceAlert> HandleAsync(RaiseAdherenceAlertCommand command)
    {
        var alert = new AdherenceAlert(command.PatientId, command.Severity, command.Reason);
        return await _alertRepository.AddAsync(alert);
    }

    public async Task<AdherenceAlert> HandleAsync(AcknowledgeAlertCommand command)
    {
        var alert = await _alertRepository.FindByIdAsync(command.AlertId);
        if (alert == null)
            throw new ArgumentException($"Alert with id {command.AlertId} not found");

        alert.Acknowledge();
        return await _alertRepository.UpdateAsync(alert);
    }

    public async Task<bool> HasOpenAlertAsync(int patientId)
    {
        var alerts = await _alertRepository.FindByPatientIdAsync(patientId);
        return alerts.Any(a => a.Status.IsOpen);
    }
}
