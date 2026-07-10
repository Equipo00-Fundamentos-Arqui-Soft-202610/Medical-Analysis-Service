using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/dashboards")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardQueryService _queryService;
    private readonly DashboardResourceFromAggregateAssembler _assembler;

    public DashboardController(
        IDashboardQueryService queryService,
        DashboardResourceFromAggregateAssembler assembler)
    {
        _queryService = queryService;
        _assembler = assembler;
    }

    [HttpGet("adherence-trend")]
    public async Task<ActionResult<AdherenceTrendResource>> GetAdherenceTrend(
        [FromQuery] int patientId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        try
        {
            var metrics = await _queryService.HandleAsync(
                new GetAdherenceTrendByPatientIdQuery(patientId, from, to));

            if (!metrics.Any())
                return NotFound(new { message = $"No adherence data found for patient {patientId}" });

            return Ok(_assembler.ToResource(patientId, metrics));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("all-adherence")]
    public async Task<ActionResult<AllAdherenceMetricsResponse>> GetAllAdherence()
    {
        var metrics = await _queryService.HandleAsync(new GetAllAdherenceMetricsQuery());
        return Ok(_assembler.ToAllResource(metrics));
    }
}
