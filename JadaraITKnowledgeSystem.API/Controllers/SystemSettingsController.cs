using JadaraITKnowledgeSystem.API.Contracts;
using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[ApiController]
[Route("api/system/settings")]
[Authorize(Roles = Roles.AdminOrAbove)]
public sealed class SystemSettingsController(IFeatureFlagService featureFlags) : ControllerBase
{
    /// <summary>Lists all feature flags.</summary>
    [HttpGet("feature-flags")]
    [ProducesResponseType<FeatureFlagsDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatureFlags(CancellationToken cancellationToken) =>
        Ok(await featureFlags.GetAllFlagsAsync(cancellationToken));

    /// <summary>Turns AI quiz generation on or off.</summary>
    [HttpPut("feature-flags/quiz-generation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ToggleQuizGeneration([FromBody] ToggleFeatureRequest request, CancellationToken cancellationToken)
    {
        await featureFlags.ToggleQuizGenerationAsync(request.Enabled, cancellationToken);
        return NoContent();
    }

    /// <summary>Whether AI quiz generation is enabled (public, so the UI can hide the feature).</summary>
    [HttpGet("feature-flags/quiz-generation")]
    [AllowAnonymous]
    [ProducesResponseType<FeatureStatusResponse>(StatusCodes.Status200OK)]
    public IActionResult IsQuizGenerationEnabled() =>
        Ok(new FeatureStatusResponse(featureFlags.IsQuizGenerationEnabled()));
}
