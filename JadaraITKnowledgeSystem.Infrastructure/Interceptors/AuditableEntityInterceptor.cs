using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Interceptors
{
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly TimeProvider _timeProvider;

        public AuditableEntityInterceptor(ICurrentUserService currentUserService, TimeProvider timeProvider)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        // Sync version
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateAuditEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        // Async version
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateAuditEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditEntities(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries<AuditableEntity>();

            // Get user email from current user service, fallback to "System" for background jobs/seeds
            var userEmail = _currentUserService.Email;
            var user = !string.IsNullOrWhiteSpace(userEmail) ? userEmail : "System";

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated(user);
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetUpdated(user);
                }
            }
        }
    }
}
