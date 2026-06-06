using Microsoft.EntityFrameworkCore;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC.Configuration;

public class MedicalAnalysisDbContext : DbContext
{
    public MedicalAnalysisDbContext(DbContextOptions<MedicalAnalysisDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
