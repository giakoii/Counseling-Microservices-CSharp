using AppointmentService.Application.CounselorSchedules.Queries;
using Common;
using Common.Utils.Const;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace AppointmentService.API.Controllers;

/// <summary>
/// CounselorScheduleController - Controller for managing counselor schedules.
/// </summary>
[ApiController]
[Route("api/v1/counselor-schedule")]
//[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class CounselorScheduleController : ControllerBase
{
    private readonly IMediator _mediator;

    public CounselorScheduleController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Select all counselor schedules with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> SelectCounselorSchedulesAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new SelectCounselorSchedulesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        
        var result = await _mediator.Send(query);
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
    
    /// <summary>
    /// Select counselor schedule by ID.
    /// </summary>
    /// <param name="id">Schedule ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> SelectCounselorScheduleByIdAsync([FromRoute] Guid id)
    {
        var query = new SelectCounselorScheduleByIdQuery
        {
            ScheduleId = id
        };
        
        var result = await _mediator.Send(query);
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
}