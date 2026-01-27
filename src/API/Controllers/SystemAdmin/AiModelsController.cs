using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.AiModels.Queries.GetAiModelMetrics;
using Application.SystemAdmin.AiModels.Queries.GetModelVersions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin AI Model Monitoring endpoints
/// Provides real-time visibility into AI model performance, operational health, and fairness indicators.
/// Note: AI model management is not yet implemented - these are placeholder endpoints
/// </summary>
[Route("api/system-admin/ai-models")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class AiModelsController : BaseApiController
{
    private readonly IMediator _mediator;

    public AiModelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get AI model performance metrics
    /// </summary>
    /// <remarks>
    /// Screen: 3.9.1-3.9.4 View AI Model Monitoring Overview, Global Accuracy, False Positive Rate, Inference Time
    /// </remarks>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(ApiResponse<AiModelMetricsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMetrics()
    {
        var result = await _mediator.Send(new GetAiModelMetricsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Get model version history
    /// </summary>
    /// <param name="limit">Number of versions to retrieve (default: 10)</param>
    /// <remarks>
    /// Screen: 3.9.7 View Model Version History
    /// </remarks>
    [HttpGet("versions")]
    [ProducesResponseType(typeof(ApiResponse<ModelVersionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetVersions([FromQuery] int limit = 10)
    {
        var query = new GetModelVersionsQuery { Limit = limit };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Promote a model version (mock endpoint)
    /// </summary>
    /// <param name="id">Model version ID</param>
    /// <param name="request">Promotion target status</param>
    /// <remarks>
    /// Screen: 3.9.8 Promote Model Version
    /// Note: Mock implementation - AI model management not yet implemented
    /// </remarks>
    [HttpPost("{id:guid}/promote")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public IActionResult PromoteModel(Guid id, [FromBody] PromoteModelRequest request)
    {
        // Mock response
        return Ok(ApiResponseFactory.Success(new { success = true }, $"Model promoted to {request.TargetStatus} successfully"));
    }

    /// <summary>
    /// Deploy a new AI model version (mock endpoint)
    /// </summary>
    /// <param name="request">Model deployment data</param>
    /// <remarks>
    /// Screen: 3.9.9 Deploy New Model
    /// Note: Mock implementation - AI model management not yet implemented
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public IActionResult DeployModel([FromBody] DeployModelRequest request)
    {
        // Mock response
        var newId = Guid.NewGuid();
        return CreatedAtAction(nameof(GetVersions), null, ApiResponseFactory.Success(newId, "Model deployed successfully"));
    }
}

/// <summary>
/// Request model for promoting a model version
/// </summary>
public class PromoteModelRequest
{
    public string TargetStatus { get; set; } = string.Empty;
}

/// <summary>
/// Request model for deploying a new model
/// </summary>
public class DeployModelRequest
{
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ReleaseNotes { get; set; }
}
