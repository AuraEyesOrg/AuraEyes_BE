using Application.Common.Models;
using Application.Ophthalmologists.Commands.CreateOphthalmologist;
using Application.Ophthalmologists.Commands.UpdateOphthalmologist;
using Application.Ophthalmologists.Queries.GetOphthalmologist;
using Application.Ophthalmologists.Queries.GetOphthalmologists;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Ophthalmologists management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OphthalmologistsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OphthalmologistsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get ophthalmologists with pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<OphthalmologistListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<PagedResult<OphthalmologistListDto>>>> GetOphthalmologists(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isVerified = null,
        [FromQuery] int? minYearsOfExperience = null,
        [FromQuery] int? maxYearsOfExperience = null)
    {
        var query = new GetOphthalmologistsQuery 
        { 
            PageNumber = pageNumber, 
            PageSize = pageSize,
            IsVerified = isVerified,
            MinYearsOfExperience = minYearsOfExperience,
            MaxYearsOfExperience = maxYearsOfExperience
        };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Get ophthalmologist by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<OphthalmologistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<OphthalmologistDto>>> GetOphthalmologist(Guid id)
    {
        var query = new GetOphthalmologistQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Create a new ophthalmologist
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<Guid>>> CreateOphthalmologist([FromBody] CreateOphthalmologistCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetOphthalmologist), new { id = result.Data }, result);
    }

    /// <summary>
    /// Update an existing ophthalmologist
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UpdateOphthalmologist(Guid id, [FromBody] UpdateOphthalmologistCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(Result.Failure("ID in URL does not match ID in request body"));
        }

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
