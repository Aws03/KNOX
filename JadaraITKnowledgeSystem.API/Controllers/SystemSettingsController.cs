using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[ApiController]
[Route("api/system/settings")]
[Authorize(Roles = "Admin")]
public class SystemSettingsController : ControllerBase
{
    private readonly IFeatureFlagService _featureFlagService;

    public SystemSettingsController(IFeatureFlagService featureFlagService)
    {
        _featureFlagService = featureFlagService;
    }

    /// <summary>
    /// Get all feature flags
    /// </summary>
    [HttpGet("feature-flags")]
    [ProducesResponseType(typeof(FeatureFlagsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatureFlags(CancellationToken cancellationToken)
    {
        var flags = await _featureFlagService.GetAllFlagsAsync(cancellationToken);
        return Ok(flags);
    }

    /// <summary>
    /// Toggle quiz generation feature on/off
    /// </summary>
    [HttpPut("feature-flags/quiz-generation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ToggleQuizGeneration(
        [FromBody] ToggleFeatureRequest request,
        CancellationToken cancellationToken)
    {
        await _featureFlagService.ToggleQuizGenerationAsync(request.Enabled, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Check if quiz generation is enabled
    /// </summary>
    [HttpGet("feature-flags/quiz-generation")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FeatureStatusResponse), StatusCodes.Status200OK)]
    public IActionResult IsQuizGenerationEnabled()
    {
        var isEnabled = _featureFlagService.IsQuizGenerationEnabled();
        return Ok(new FeatureStatusResponse { Enabled = isEnabled });
    }
}

public record ToggleFeatureRequest(bool Enabled);
public record FeatureStatusResponse { public bool Enabled { get; init; } }
