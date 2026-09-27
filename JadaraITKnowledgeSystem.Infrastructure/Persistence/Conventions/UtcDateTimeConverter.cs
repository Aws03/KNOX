using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Conventions;

/// <summary>
/// All DateTime values are written as UTC; this marks them as UTC when read back, so they
/// serialize with a zone ("Z") instead of being taken as local time by clients.
/// </summary>
public sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    v => v.Kind == DateTimeKind.Local ? v.ToUniversalTime() : DateTime.SpecifyKind(v, DateTimeKind.Utc),
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
