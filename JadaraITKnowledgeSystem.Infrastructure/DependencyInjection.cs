using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Infrastructure.Identity;
using JadaraITKnowledgeSystem.Infrastructure.Interceptors;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using JadaraITKnowledgeSystem.Infrastructure.Persistence;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Seed;
using JadaraITKnowledgeSystem.Infrastructure.Services.AI;
using JadaraITKnowledgeSystem.Infrastructure.Services.BackgroundJobs;
using JadaraITKnowledgeSystem.Infrastructure.Services.Email;
using JadaraITKnowledgeSystem.Infrastructure.Services.FileManagement;
using JadaraITKnowledgeSystem.Infrastructure.Services.JWT;
using JadaraITKnowledgeSystem.Infrastructure.Services.Security;
using JadaraITKnowledgeSystem.Infrastructure.Services.Storage;
using JadaraITKnowledgeSystem.Infrastructure.Services.FeatureFlags;
using JadaraITKnowledgeSystem.Infrastructure.Services.TextExtraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public const string ConnectionStringName = "DefaultConnection";

        /// <remarks>
        /// Nothing here reads configuration eagerly: every setting is bound through the options
        /// system and resolved when first used, so hosts (and tests) can layer configuration freely.
        /// </remarks>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton(TimeProvider.System);
            services.AddValidatedOptions<JwtOptions>(JwtOptions.SectionName);
            services.AddValidatedOptions<StorageOptions>(StorageOptions.SectionName);
            services.AddOptions<OpenAIOptions>().BindConfiguration(OpenAIOptions.SectionName);
            services.AddOptions<BrevoOptions>().BindConfiguration(BrevoOptions.SectionName);
            services.AddOptions<AhaSendOptions>().BindConfiguration(AhaSendOptions.SectionName);
            services.AddValidatedOptions<DatabaseOptions>(DatabaseOptions.SectionName);
            services.AddOptions<SeedOptions>().BindConfiguration(SeedOptions.SectionName);

            AddPersistence(services);

            // Identity, tokens and the current caller
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IIdentityUserService, IdentityUserService>();
            services.AddScoped<IIdentityRoleService, IdentityRoleService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IOTPService, OTPService>();

            AddEmail(services);

            // Files
            // The S3 client is thread-safe and pools connections: one per process.
            services.AddSingleton<S3StorageService>();
            services.AddSingleton<IStorageService>(sp => sp.GetRequiredService<S3StorageService>());
            services.AddScoped<IFileManager, FileManager>();
            services.AddHostedService<TempFileCleanupJob>();

            // Quiz generation
            services.AddScoped<ITextExtractionService, TextExtractionService>();
            services.AddHttpClient<IOpenAIService, OpenAIService>(client => client.Timeout = TimeSpan.FromMinutes(2));
            services.AddScoped<IFeatureFlagService, FeatureFlagService>();
            services.AddMemoryCache();

            // Post-commit background jobs. PostCommitDispatcher is a per-request staging area
            // (drained by DispatchPostCommitJobsBehavior after the transaction commits);
            // BackgroundJobQueue is the process-wide queue QueuedBackgroundService consumes.
            services.AddScoped<IPostCommitDispatcher, PostCommitDispatcher>();
            services.AddSingleton<BackgroundJobQueue>();
            services.AddSingleton<IBackgroundJobQueue>(sp => sp.GetRequiredService<BackgroundJobQueue>());
            services.AddHostedService<QueuedBackgroundService>();

            return services;
        }

        private static void AddPersistence(IServiceCollection services)
        {
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString(ConnectionStringName);
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException($"ConnectionStrings:{ConnectionStringName} is not configured.");

                options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(maxRetryCount: 5))
                       .AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
            });
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            services.AddScoped<RoleSeeder>();
            services.AddScoped<DataSeeder>();
            services.AddScoped<DatabaseInitializer>();
        }

        private static void AddEmail(IServiceCollection services)
        {
            services.AddHttpClient<BrevoEmailService>();
            services.AddHttpClient<AhaSendEmailService>();
            services.AddScoped<LoggingEmailService>();

            // Brevo first, then AhaSend; with neither configured emails are only logged.
            services.AddScoped<IEmailService>(sp =>
                sp.GetRequiredService<IOptions<BrevoOptions>>().Value.IsConfigured ? sp.GetRequiredService<BrevoEmailService>()
                : sp.GetRequiredService<IOptions<AhaSendOptions>>().Value.IsConfigured ? sp.GetRequiredService<AhaSendEmailService>()
                : sp.GetRequiredService<LoggingEmailService>());
        }

        private static void AddValidatedOptions<TOptions>(this IServiceCollection services, string section)
            where TOptions : class =>
            services.AddOptions<TOptions>()
                .BindConfiguration(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();
    }
}
