using MediTrack.MedicalAnalysisService.API.Domain.Model.Aggregates;
using MediTrack.MedicalAnalysisService.API.Domain.Model.ValueObjects;
using MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC.Configuration;

public class MedicalAnalysisDbContext : DbContext
{
    public MedicalAnalysisDbContext(DbContextOptions<MedicalAnalysisDbContext> options)
        : base(options) { }

    public DbSet<AdherenceMetric> AdherenceMetrics { get; set; } = null!;
    public DbSet<ComplianceStatistic> ComplianceStatistics { get; set; } = null!;
    public DbSet<AdherenceAlert> AdherenceAlerts { get; set; } = null!;
    public DbSet<ClinicalRecord> ClinicalRecords { get; set; } = null!;
    public DbSet<ProcessedEvent> ProcessedEvents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AdherenceMetric>(entity =>
        {
            entity.ToTable("adherence_metric");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PatientId)
                .HasColumnName("patient_id")
                .IsRequired();

            entity.Property(e => e.Category)
                .HasColumnName("category")
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(v => v.Value, v => ComplianceCategory.From(v));

            entity.Property(e => e.TotalScheduled)
                .HasColumnName("total_scheduled")
                .IsRequired();

            entity.Property(e => e.TotalCompliant)
                .HasColumnName("total_compliant")
                .IsRequired();

            entity.Property(e => e.TotalMissed)
                .HasColumnName("total_missed")
                .IsRequired();

            entity.Property(e => e.Rate)
                .HasColumnName("rate")
                .HasPrecision(5, 2)
                .IsRequired()
                .HasConversion(v => v.Value, v => AdherenceRate.From(v));

            entity.Property(e => e.LastUpdatedAt)
                .HasColumnName("last_updated_at")
                .IsRequired();

            entity.HasIndex(e => new { e.PatientId, e.Category }).IsUnique();
        });

        modelBuilder.Entity<ComplianceStatistic>(entity =>
        {
            entity.ToTable("compliance_statistic");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Category)
                .HasColumnName("category")
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(v => v.Value, v => ComplianceCategory.From(v));

            entity.Property(e => e.WindowStart)
                .HasColumnName("window_start")
                .IsRequired();

            entity.Property(e => e.WindowEnd)
                .HasColumnName("window_end")
                .IsRequired();

            entity.Property(e => e.TotalScheduled)
                .HasColumnName("total_scheduled")
                .IsRequired();

            entity.Property(e => e.TotalCompliant)
                .HasColumnName("total_compliant")
                .IsRequired();

            entity.Property(e => e.TotalMissed)
                .HasColumnName("total_missed")
                .IsRequired();

            entity.Property(e => e.ComplianceRate)
                .HasColumnName("compliance_rate")
                .HasPrecision(5, 2)
                .IsRequired()
                .HasConversion(v => v.Value, v => AdherenceRate.From(v));

            entity.Property(e => e.CalculatedAt)
                .HasColumnName("calculated_at")
                .IsRequired();
        });

        modelBuilder.Entity<AdherenceAlert>(entity =>
        {
            entity.ToTable("adherence_alert");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PatientId)
                .HasColumnName("patient_id")
                .IsRequired();

            entity.Property(e => e.Severity)
                .HasColumnName("severity")
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(v => v.Value, v => AlertSeverity.From(v));

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(v => v.Value, v => AlertStatus.From(v));

            entity.Property(e => e.Reason)
                .HasColumnName("reason")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.TriggeredAt)
                .HasColumnName("triggered_at")
                .IsRequired();

            entity.Property(e => e.AcknowledgedAt)
                .HasColumnName("acknowledged_at");
        });

        modelBuilder.Entity<ClinicalRecord>(entity =>
        {
            entity.ToTable("clinical_record");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PatientId)
                .HasColumnName("patient_id")
                .IsRequired();

            entity.Property(e => e.RecordDate)
                .HasColumnName("record_date")
                .IsRequired();

            entity.Property(e => e.Diagnosis)
                .HasColumnName("diagnosis")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.Notes)
                .HasColumnName("notes")
                .HasMaxLength(2000);

            entity.Property(e => e.Source)
                .HasColumnName("source")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.ImportBatchId)
                .HasColumnName("import_batch_id")
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
        });

        modelBuilder.Entity<ProcessedEvent>(entity =>
        {
            entity.ToTable("processed_events");

            entity.HasKey(e => e.EventId);

            entity.Property(e => e.EventId)
                .HasColumnName("event_id")
                .HasConversion(g => g.ToByteArray(), b => new Guid(b))
                .HasColumnType("binary(16)");

            entity.Property(e => e.EventType)
                .HasColumnName("event_type")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.ProcessedAtUtc)
                .HasColumnName("processed_at_utc")
                .IsRequired();
        });
    }
}
