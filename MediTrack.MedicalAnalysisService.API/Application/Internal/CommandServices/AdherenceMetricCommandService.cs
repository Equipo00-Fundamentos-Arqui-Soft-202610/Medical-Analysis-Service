using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Services;
using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.CommandServices;

public class AdherenceMetricCommandService : IAdherenceMetricCommandService
{
    private readonly IAdherenceMetricRepository _metricRepository;
    private readonly IAlertCommandService _alertCommandService;
    private readonly AdherenceCalculatorFactory _calculatorFactory;

    public AdherenceMetricCommandService(
        IAdherenceMetricRepository metricRepository,
        IAlertCommandService alertCommandService,
        AdherenceCalculatorFactory calculatorFactory)
    {
        _metricRepository = metricRepository;
        _alertCommandService = alertCommandService;
        _calculatorFactory = calculatorFactory;
    }

    public async Task<AdherenceMetric> HandleAsync(RecalculateAdherenceMetricCommand command)
    {
        var category = ComplianceCategory.From(command.Category);
        var calculator = _calculatorFactory.GetCalculator(category);

        var metric = await _metricRepository.FindByPatientIdAndCategoryAsync(command.PatientId, command.Category);

        if (metric == null)
        {
            metric = new AdherenceMetric(command.PatientId, command.Category);
            metric.RegisterEvent(command.WasCompliant);
            metric = await _metricRepository.AddAsync(metric);
        }
        else
        {
            metric.RegisterEvent(command.WasCompliant);
            metric = await _metricRepository.UpdateAsync(metric);
        }

        if (calculator.ShouldRaiseAlert(metric.Rate))
        {
            var severity = calculator.DetermineAlertSeverity(metric.Rate);
            var reason = $"Adherence rate dropped to {metric.Rate.Value:F1}% (threshold: " +
                         $"{(command.Category == "medication" ? "70" : "80")}%)";

            await _alertCommandService.HandleAsync(new RaiseAdherenceAlertCommand(
                command.PatientId, severity, reason));
        }

        return metric;
    }
}
