using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Infrastructure.Interceptors;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Services.BackgroundJobs;
using JadaraITKnowledgeSystem.Infrastructure.Services.FileManagement;
using JadaraITKnowledgeSystem.Infrastructure.Services.FileMangment;
using JadaraITKnowledgeSystem.Infrastructure.Services.JWT;
using JadaraITKnowledgeSystem.Infrastructure.Services.Security;
using JadaraITKnowledgeSystem.Infrastructure.Services.Email;
using JadaraITKnowledgeSystem.Infrastructure.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JadaraITKnowledgeSystem.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(TimeProvider.System);

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            ArgumentNullException.ThrowIfNullOrEmpty(connectionString);

            // Register the AuditableEntityInterceptor as scoped
            services.AddScoped<AuditableEntityInterceptor>();

            // Register DbContext with interceptor
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var interceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();
                options.UseSqlServer(connectionString)
                       .AddInterceptors(interceptor);
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            services.AddHostedService<TempFileCleanupJob>();

            services.AddScoped<IFileManager, FileManager>();
            services.AddScoped<IStorageService, LocalFileStorage>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IIdentityUserService, Identity.IdentityUserService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            // Brevo is the primary email provider; fall back to AhaSend if Brevo isn't configured.
            if (!string.IsNullOrWhiteSpace(configuration["Brevo:ApiKey"]))
                services.AddHttpClient<IEmailService, BrevoEmailService>();
            else
                services.AddHttpClient<IEmailService, EmailService>();

            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IIdentityRoleService, Identity.IdentityRoleService>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Post-commit background job dispatch (replaces Task.Run + Task.Delay(100)).
            // PostCommitDispatcher is scoped (per-request staging area, drained by
            // DispatchPostCommitJobsBehavior after the request's transaction commits).
            // BackgroundJobQueue is a process-wide singleton; it's registered under its
            // concrete type too so QueuedBackgroundService can consume it directly
            // while command handlers only ever see it through the IBackgroundJobQueue port.
            services.AddScoped<IPostCommitDispatcher, PostCommitDispatcher>();
            services.AddSingleton<BackgroundJobQueue>();
            services.AddSingleton<IBackgroundJobQueue>(sp => sp.GetRequiredService<BackgroundJobQueue>());
            services.AddHostedService<QueuedBackgroundService>();

            return services;
        }
    }
}
