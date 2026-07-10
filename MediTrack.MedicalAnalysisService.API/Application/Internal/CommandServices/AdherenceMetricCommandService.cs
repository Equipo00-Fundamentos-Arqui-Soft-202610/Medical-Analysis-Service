using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Commands;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Services;
using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

namespace MediTrack.MedicalAnalysisService.API.Application.Internal.CommandServices;

public class AdherenceMetricCommandService : IAdherenceMetricCommandService
{
    private readonly IAdherenceMetricRepository _metricRepository;
    private readonly IComplianceStatisticRepository _statisticRepository;
    private readonly IAlertCommandService _alertCommandService;
    private readonly AdherenceCalculatorFactory _calculatorFactory;

    public AdherenceMetricCommandService(
        IAdherenceMetricRepository metricRepository,
        IComplianceStatisticRepository statisticRepository,
        IAlertCommandService alertCommandService,
        AdherenceCalculatorFactory calculatorFactory)
    {
        _metricRepository = metricRepository;
        _statisticRepository = statisticRepository;
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

        await RegisterDailyStatisticAsync(command.Category, command.WasCompliant);

        if (calculator.ShouldRaiseAlert(metric.Rate))
        {
            await RaiseAlertIfNotAlreadyOpenAsync(command, calculator, metric);
        }

        return metric;
    }

    /// <summary>
    /// Acumula el evento en el bucket diario de ComplianceStatistic (US18 -- sin
    /// esto, /statistics/* siempre devolvía vacío porque nada escribía esa tabla).
    /// </summary>
    private async Task RegisterDailyStatisticAsync(string category, bool wasCompliant)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var existing = (await _statisticRepository.FindByCategoryAndRangeAsync(category, today, tomorrow))
            .FirstOrDefault();

        var statistic = existing ?? new ComplianceStatistic(category, today, tomorrow);

        var totalCompliant = statistic.TotalCompliant + (wasCompliant ? 1 : 0);
        var totalMissed = statistic.TotalMissed + (wasCompliant ? 0 : 1);
        statistic.Update(statistic.TotalScheduled + 1, totalCompliant, totalMissed);

        if (existing == null)
            await _statisticRepository.AddAsync(statistic);
        else
            await _statisticRepository.UpdateAsync(statistic);
    }

    /// <summary>
    /// US-AC: sin esta verificación, un paciente crónicamente no-adherente genera
    /// una alerta nueva en CADA evento mientras se mantenga bajo el umbral.
    /// </summary>
    private async Task RaiseAlertIfNotAlreadyOpenAsync(
        RecalculateAdherenceMetricCommand command, IAdherenceCalculator calculator, AdherenceMetric metric)
    {
        var hasOpenAlert = await _alertCommandService.HasOpenAlertAsync(command.PatientId);
        if (hasOpenAlert)
            return;

        var severity = calculator.DetermineAlertSeverity(metric.Rate);
        var reason = $"Adherence rate dropped to {metric.Rate.Value:F1}% (threshold: " +
                     $"{(command.Category == "medication" ? "70" : "80")}%)";

        await _alertCommandService.HandleAsync(new RaiseAdherenceAlertCommand(
            command.PatientId, severity, reason));
    }
}
