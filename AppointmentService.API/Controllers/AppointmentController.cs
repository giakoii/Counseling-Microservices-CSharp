using AppointmentService.Application.Appointments.Commands;
using AppointmentService.Application.Appointments.Queries;
using Common;
using Common.Utils.Const;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace AppointmentService.API.Controllers;

/// <summary>
/// AppointmentController - Controller for managing appointments.
/// </summary>
[ApiController]
[Route("api/v1/appointment")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class AppointmentController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mediator"></param>
    public AppointmentController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Insert a new appointment.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> InsertAppointment([FromBody] AppointmentInsertCommand request)
    {
        var response = new AppointmentInsertResponse { Success = false };
        var errorList = AbstractFunction<AppointmentInsertResponse, AppointmentInsertEntity>.ErrorCheck(ModelState);
        if (errorList.Count > 0)
        {
            response.SetMessage(MessageId.E10000);
            return BadRequest(response);
        }
        response = await _mediator.Send(request);
        if (response.Success)
        {
            return Created(response.AppointmentId, response);
        }
        
        return BadRequest(response);
    }
    
    /// <summary>
    /// Select all appointments with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> SelectAppointmentsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new AppointmentSelectsQuery
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
    /// Select appointment by ID.
    /// </summary>
    /// <param name="id">Appointment ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> SelectAppointmentByAppointmentIdAsync([FromRoute] Guid id)
    {
        var query = new AppointmentSelectByIdQuery
        {
            AppointmentId = id
        };
        
        var result = await _mediator.Send(query);
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
    
    /// <summary>
    /// Select appointments by user ID with pagination.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns></returns>
    [HttpGet("user/{id}")]
    public async Task<IActionResult> SelectAppointmentByUserIdAsync([FromRoute] Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new AppointmentSelectByUserIdQuery
        {
            UserId = userId,
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
    /// Select appointment statistics for the last 6 months and today.
    /// </summary>
    /// <returns></returns>
    [HttpGet("statistics/6-months")]
    public async Task<IActionResult> SelectAppointmentIn6MonthsAsync()
    {
        var query = new AppointmentSelectIn6MonthsQuery();
        
        var result = await _mediator.Send(query);
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
    
    /// <summary>
    /// Update the status of an appointment.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateAppointmentStatusAsync([FromRoute] Guid id, [FromBody] AppointmentUpdateStatusCommand request)
    {
        var response = new BaseCommandResponse { Success = false };
        request.AppointmentId = id;

        var errorList = AbstractFunction<BaseCommandResponse, string>.ErrorCheck(ModelState);
        if (errorList.Count > 0)
        {
            response.SetMessage(MessageId.E10000);
            return BadRequest(response);
        }
        response = await _mediator.Send(request);
        if (response.Success)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
    
    [HttpPatch("{appointmentId}/payment-call-back")]
    public async Task<IActionResult> AppointmentPaymentCallbackAsync([FromRoute] Guid appointmentId, [FromBody] AppointmentPaymentCallbackCommand request)
    {
        var response = new AppointmentInsertResponse { Success = false };
        request.AppointmentId = appointmentId;

        var errorList = AbstractFunction<AppointmentInsertResponse, AppointmentInsertEntity>.ErrorCheck(ModelState);
        if (errorList.Count > 0)
        {
            response.SetMessage(MessageId.E10000);
            return BadRequest(response);
        }
        response = await _mediator.Send(request);
        if (response.Success)
        {
            return Ok(response);
        }
        
        return BadRequest(response);
    }
}