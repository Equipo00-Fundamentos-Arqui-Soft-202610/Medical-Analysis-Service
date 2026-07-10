using MediTrack.MedicalAnalysisService.API.Domain.Model;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Queries;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Resources;
using MediTrack.MedicalAnalysisService.API.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MediTrack.MedicalAnalysisService.API.Interfaces.REST.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsQueryService _queryService;
    private readonly StatisticsResourceFromAggregateAssembler _assembler;

    public StatisticsController(
        IStatisticsQueryService queryService,
        StatisticsResourceFromAggregateAssembler assembler)
    {
        _queryService = queryService;
        _assembler = assembler;
    }

    [HttpGet("compliance")]
    public async Task<ActionResult<IEnumerable<ComplianceStatisticResource>>> GetComplianceStatistics(
        [FromQuery] string category,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        try
        {
            var results = await _queryService.HandleAsync(new GetComplianceStatisticsQuery(category, from, to));
            if (!results.Any())
                return NotFound(new { message = "No compliance statistics found for the given parameters" });

            return Ok(_assembler.ToResources(results));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("appointments")]
    public async Task<ActionResult<IEnumerable<ComplianceStatisticResource>>> GetAppointmentStatistics(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        try
        {
            var results = await _queryService.HandleAsync(new GetAppointmentStatisticsQuery(from, to));
            if (!results.Any())
                return NotFound(new { message = "No appointment statistics found for the given range" });

            return Ok(_assembler.ToResources(results));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
