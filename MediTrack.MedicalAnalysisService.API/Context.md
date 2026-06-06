# MediTrack - Medical Analysis Service Context

## Stack

- ASP.NET Core 8 Web API
- Microsoft.EntityFrameworkCore 8.0.8
- MySql.EntityFrameworkCore 8.0.8
- Swashbuckle.AspNetCore 6.6.2
- Clean Architecture / DDD + CQRS
- .NET SDK 9.0.303

## Structure

MedicalAnalysisService/
- Domain/Model/Aggregates/         — AdherenceMetric, ComplianceStatistic, AdherenceAlert, ClinicalRecord
- Domain/Model/ValueObjects/       — AdherenceRate, AlertSeverity, AlertStatus, ComplianceCategory, DateRange
- Domain/Model/Commands/           — RegisterClinicalRecord, ImportClinicalDataset, RecalculateAdherenceMetric, RaiseAdherenceAlert, AcknowledgeAlert
- Domain/Model/Queries/            — GetAdherenceTrendByPatientId, GetComplianceStatistics, GetAppointmentStatistics, GetActiveAlerts, GetClinicalHistoryByPatientId
- Domain/Model/Events/             — ComplianceRegistered, AppointmentAttendanceRegistered, PrescriptionLoaded (integration events)
- Domain/Model/Services/           — IAdherenceCalculator (Strategy), MedicationAdherenceStrategy, AppointmentAdherenceStrategy, AdherenceCalculatorFactory
- Application/Internal/CommandServices/  — ClinicalDataCommandService, AdherenceMetricCommandService, AlertCommandService
- Application/Internal/QueryServices/   — StatisticsQueryService, DashboardQueryService, AlertQueryService, ClinicalDataQueryService
- Application/Internal/EventHandlers/   — ComplianceRegisteredEventHandler, AppointmentAttendanceRegisteredEventHandler, PrescriptionLoadedEventHandler
- Infrastructure/Persistence/EFC/       — MedicalAnalysisDbContext, 4 repositories
- Infrastructure/Messaging/             — IIntegrationEventBus, InMemoryEventBus (Sprint 1), HostedEventConsumer
- Infrastructure/DatasetProcessing/     — IDatasetProcessor, CsvDatasetProcessor
- Interfaces/REST/Controllers/          — ClinicalDataController, StatisticsController, DashboardController, AlertController
- Interfaces/REST/Resources/            — ClinicalDataResources, StatisticsResources, DashboardResources, AlertResources
- Interfaces/REST/Transform/            — 6 assemblers

## Implemented requirements

- US14: Personal Técnico carga historiales clínicos mediante datasets CSV (ClinicalDataController — POST /import).
- US16: Personal Técnico visualiza gráficos de tendencia de adherencia por paciente (DashboardController — GET /dashboards/adherence-trend).
- US18: Personal Técnico consulta estadísticas de cumplimiento de recetas y tipos de cita (StatisticsController — GET /statistics/compliance, GET /statistics/appointments).
- AC-10 (CQRS): separación de CommandServices y QueryServices para aislar la carga analítica del flujo transaccional.
- AC-08 (Eventual consistency): AdherenceMetric se actualiza de forma asíncrona al consumir ComplianceRegisteredIntegrationEvent del bus.
- Strategy Pattern: IAdherenceCalculator con MedicationAdherenceStrategy (umbral 70%) y AppointmentAdherenceStrategy (umbral 80%); selección vía AdherenceCalculatorFactory.
- Alertas automáticas: AdherenceMetricCommandService dispara RaiseAdherenceAlertCommand cuando la tasa cae bajo el umbral.

## Endpoints (api/v1)

| Method | Route                                      | Description                          |
|--------|--------------------------------------------|--------------------------------------|
| POST   | /clinical-records                          | Registra un historial clínico manual |
| POST   | /clinical-records/import                   | Importa dataset CSV (US14)           |
| GET    | /clinical-records?patientId={id}           | Historial clínico por paciente       |
| GET    | /statistics/compliance?category&from&to    | Estadísticas de cumplimiento (US18)  |
| GET    | /statistics/appointments?from&to           | Estadísticas de citas (US18)         |
| GET    | /dashboards/adherence-trend?patientId&from&to | Tendencia de adherencia (US16)    |
| GET    | /alerts?status={s}                         | Listar alertas por estado            |
| POST   | /alerts                                    | Crear alerta manualmente             |
| PATCH  | /alerts/{id}/acknowledge                   | Reconocer una alerta                 |

## Database

- Connection name: `AnalysisDB`
- Hostname: `127.0.0.1`
- Port: `3306`
- Username: `root`
- Database: `AnalysisDB`
