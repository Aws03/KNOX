using System;
using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.System.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.System;

public class FeatureFlagService : IFeatureFlagService
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<FeatureFlagService> _logger;
    
    private const string CACHE_KEY_PREFIX = "FeatureFlag_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private const string QUIZ_GENERATION_ENABLED_KEY = "Features.QuizGeneration.Enabled";
    private const string MAX_CONCURRENT_JOBS_KEY = "Features.QuizGeneration.MaxConcurrent";
    private const string DEFAULT_QUESTIONS_KEY = "Features.QuizGeneration.DefaultQuestions";

    public FeatureFlagService(
        IApplicationDbContext context,
        IMemoryCache cache,
        ILogger<FeatureFlagService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public bool IsQuizGenerationEnabled()
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}QuizGeneration";

        if (_cache.TryGetValue<bool>(cacheKey, out var cachedValue))
        {
            return cachedValue;
        }

        try
        {
            var setting = _context.SystemSettings
                .AsNoTracking()
                .FirstOrDefault(s => s.Key == QUIZ_GENERATION_ENABLED_KEY);

            var isEnabled = setting?.IsEnabled == true &&
                           bool.TryParse(setting.Value, out var value) && value;

            _cache.Set(cacheKey, isEnabled, CacheDuration);
            return isEnabled;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking quiz generation feature flag");
            return false; // Fail-safe: disabled if error
        }
    }

    public async Task<bool> ToggleQuizGenerationAsync(bool enabled, CancellationToken cancellationToken = default)
    {
        try
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == QUIZ_GENERATION_ENABLED_KEY, cancellationToken);

            if (setting == null)
            {
                var createResult = SystemSetting.Create(
                    QUIZ_GENERATION_ENABLED_KEY,
                    enabled.ToString().ToLower(),
                    "Enable AI-powered quiz generation from course materials",
                    enabled);

                if (createResult.IsError)
                {
                    _logger.LogError("Failed to create quiz generation setting: {Errors}", createResult.Errors);
                    return false;
                }

                _context.SystemSettings.Add(createResult.Value);
            }
            else
            {
                setting.UpdateValue(enabled.ToString().ToLower());
                if (enabled)
                    setting.Enable();
                else
                    setting.Disable();
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            _cache.Remove($"{CACHE_KEY_PREFIX}QuizGeneration");

            _logger.LogInformation("Quiz generation feature toggled to {Enabled}", enabled);
            return enabled;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling quiz generation feature");
            return false;
        }
    }

    public async Task<FeatureFlagsDto> GetAllFlagsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = await _context.SystemSettings
                .AsNoTracking()
                .Where(s => s.Key.StartsWith("Features."))
                .ToListAsync(cancellationToken);

            var quizGenerationEnabled = settings
                .FirstOrDefault(s => s.Key == QUIZ_GENERATION_ENABLED_KEY);

            var maxConcurrentJobs = settings
                .FirstOrDefault(s => s.Key == MAX_CONCURRENT_JOBS_KEY);

            var defaultQuestions = settings
                .FirstOrDefault(s => s.Key == DEFAULT_QUESTIONS_KEY);

            return new FeatureFlagsDto
            {
                QuizGenerationEnabled = quizGenerationEnabled?.IsEnabled == true &&
                                       bool.TryParse(quizGenerationEnabled.Value, out var qgValue) && qgValue,
                MaxConcurrentJobs = int.TryParse(maxConcurrentJobs?.Value, out var mcj) ? mcj : 3,
                DefaultQuestionsPerQuiz = int.TryParse(defaultQuestions?.Value, out var dq) ? dq : 8
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting feature flags");
            return new FeatureFlagsDto
            {
                QuizGenerationEnabled = false,
                MaxConcurrentJobs = 3,
                DefaultQuestionsPerQuiz = 8
            };
        }
    }

    public async Task<Result<string>> GetSettingValueAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var setting = await _context.SystemSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

            if (setting == null)
            {
                return Error.NotFound("SystemSetting.NotFound", $"Setting with key '{key}' not found");
            }

            return setting.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting setting value for key {Key}", key);
            return Error.Failure("SystemSetting.Error", $"Error retrieving setting: {ex.Message}");
        }
    }
}
