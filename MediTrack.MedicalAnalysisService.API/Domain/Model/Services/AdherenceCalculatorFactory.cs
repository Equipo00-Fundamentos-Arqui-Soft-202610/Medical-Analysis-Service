using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;

namespace MediTrack.MedicalAnalysisService.API.Domain.Model.Services;

public class AdherenceCalculatorFactory
{
    private readonly MedicationAdherenceStrategy _medicationStrategy;
    private readonly AppointmentAdherenceStrategy _appointmentStrategy;

    public AdherenceCalculatorFactory(
        MedicationAdherenceStrategy medicationStrategy,
        AppointmentAdherenceStrategy appointmentStrategy)
    {
        _medicationStrategy = medicationStrategy;
        _appointmentStrategy = appointmentStrategy;
    }

    public IAdherenceCalculator GetCalculator(ComplianceCategory category)
    {
        return category.Value switch
        {
            "medication" => _medicationStrategy,
            "appointment" => _appointmentStrategy,
            _ => throw new ArgumentException($"No calculator registered for category '{category.Value}'", nameof(category))
        };
    }
}
