using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using JadaraITKnowledgeSystem.API.ErrorHandling;
using JadaraITKnowledgeSystem.API.OpenApi;
using JadaraITKnowledgeSystem.Application.Common.Options;
using JadaraITKnowledgeSystem.Infrastructure.Identity;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Services.Storage;
using JadaraITKnowledgeSystem.Infrastructure.Services.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JadaraITKnowledgeSystem.API.Extensions;

public static class ServiceCollectionExtensions
{
    public const string FrontendCorsPolicy = "Frontend";
    public const string ReadyHealthTag = "ready";

    private static readonly string[] DefaultCorsOrigins = ["http://localhost:5173", "http://localhost:5174"];

    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                // Enums travel as names; numbers are still accepted on input.
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
            context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier);
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddOpenApi("v1", options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("database", tags: [ReadyHealthTag])
            .AddCheck<StorageHealthCheck>("storage", tags: [ReadyHealthTag]);

        services.AddIdentityAndJwt(configuration);
        services.AddFrontendCors(configuration);
        services.AddAuthRateLimiting();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            // Only enabled (ForwardedHeaders:Enabled) when the API sits behind a reverse proxy that
            // is its only ingress, e.g. the nginx container in docker-compose.
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
            options.ForwardLimit = 1;
        });

        return services;
    }

    private static void AddIdentityAndJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddOptions<AuthOptions>().BindConfiguration(AuthOptions.SectionName);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Configured from the validated JwtOptions when first needed, not while registering services.
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((options, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;
                options.RequireHttpsMetadata = false; // TLS is terminated by the reverse proxy
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = JwtTokenService.SigningKey(jwt),
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();
    }

    private static void AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy =>
        {
            var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() is { Length: > 0 } configured
                ? configured
                : DefaultCorsOrigins;

            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }));
    }

    private static void AddAuthRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                var problemDetails = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                await problemDetails.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context.HttpContext,
                    ProblemDetails = { Status = StatusCodes.Status429TooManyRequests, Title = "Too many requests", Detail = "Please wait a minute and try again." }
                });
            };
            options.AddPolicy(RateLimitPolicies.Auth, context => PerClientIp(context, permitsPerMinute: 5));
            options.AddPolicy(RateLimitPolicies.Otp, context => PerClientIp(context, permitsPerMinute: 3));
        });
    }

    private static RateLimitPartition<string> PerClientIp(HttpContext context, int permitsPerMinute) =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitsPerMinute,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
}
