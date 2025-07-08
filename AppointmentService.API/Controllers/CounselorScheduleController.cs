using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentService.API.Controllers;

/// <summary>
/// CounselorScheduleController - Controller for managing counselor schedules.
/// </summary>
[ApiController]
[Route("api/counselor-schedule")]
public class CounselorScheduleController : ControllerBase
{
    private readonly IMediator _mediator;

    public CounselorScheduleController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    // [HttpGet("[action]")]
    // public async Task<IActionResult> SelectCounselorSchedulesAsync()
    // {
    //     var result = await _mediator.Send(new SelectCounselorSchedulesQuery());
    //     if (result.Success)
    //     {
    //         return Ok(result);
    //     }
    //     
    //     return BadRequest(result);
    // }
}