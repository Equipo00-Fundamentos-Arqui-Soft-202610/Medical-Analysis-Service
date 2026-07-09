using System.Text;
using MediTrack.MedicalAnalysisService.API.Application.Internal.CommandServices;
using MediTrack.MedicalAnalysisService.API.Application.Internal.EventHandlers;
using MediTrack.MedicalAnalysisService.API.Application.Internal.QueryServices;
using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Services;
using MediTrack.MedicalAnalysisService.API.Infrastructure.DatasetProcessing;
using MediTrack.MedicalAnalysisService.API.Infrastructure.Messaging;
using MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC;
using MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC.Configuration;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSection["Key"]
    ?? throw new InvalidOperationException("Falta la clave de firma JWT en 'Jwt:Key'.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<MedicalAnalysisDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!));

// Repositories
builder.Services.AddScoped<IClinicalRecordRepository, ClinicalRecordRepository>();
builder.Services.AddScoped<IAdherenceMetricRepository, AdherenceMetricRepository>();
builder.Services.AddScoped<IComplianceStatisticRepository, ComplianceStatisticRepository>();
builder.Services.AddScoped<IAdherenceAlertRepository, AdherenceAlertRepository>();

// Command / Query services
builder.Services.AddScoped<IClinicalDataCommandService, ClinicalDataCommandService>();
builder.Services.AddScoped<IAdherenceMetricCommandService, AdherenceMetricCommandService>();
builder.Services.AddScoped<IAlertCommandService, AlertCommandService>();
builder.Services.AddScoped<IStatisticsQueryService, StatisticsQueryService>();
builder.Services.AddScoped<IDashboardQueryService, DashboardQueryService>();
builder.Services.AddScoped<IAlertQueryService, AlertQueryService>();
builder.Services.AddScoped<IClinicalDataQueryService, ClinicalDataQueryService>();

// Assemblers
builder.Services.AddScoped<ClinicalDataCommandFromResourceAssembler>();
builder.Services.AddScoped<ClinicalRecordResourceFromEntityAssembler>();
builder.Services.AddScoped<AlertCommandFromResourceAssembler>();
builder.Services.AddScoped<AlertResourceFromEntityAssembler>();
builder.Services.AddScoped<StatisticsResourceFromAggregateAssembler>();
builder.Services.AddScoped<DashboardResourceFromAggregateAssembler>();

// Strategies + Factory
builder.Services.AddScoped<MedicationAdherenceStrategy>();
builder.Services.AddScoped<AppointmentAdherenceStrategy>();
builder.Services.AddScoped<AdherenceCalculatorFactory>();

// Dataset processing
builder.Services.AddScoped<IDatasetProcessor, CsvDatasetProcessor>();

// Event handlers
builder.Services.AddScoped<ComplianceRegisteredEventHandler>();
builder.Services.AddScoped<AppointmentAttendanceRegisteredEventHandler>();
builder.Services.AddScoped<PrescriptionLoadedEventHandler>();

// Event bus
builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddHostedService<HostedEventConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MedicalAnalysisDbContext>();
    db.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();