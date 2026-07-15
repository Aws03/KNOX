using System;
using System.ComponentModel.DataAnnotations;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Domain.System.Entities;

public sealed class SystemSetting : AuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Key { get; private set; }

    [Required]
    [MaxLength(500)]
    public string Value { get; private set; }

    [MaxLength(200)]
    public string? Description { get; private set; }

    public bool IsEnabled { get; private set; }

    private SystemSetting() { }

    private SystemSetting(string key, string value, string? description = null, bool isEnabled = true)
    {
        Key = key;
        Value = value;
        Description = description;
        IsEnabled = isEnabled;
    }

    public static Result<SystemSetting> Create(string key, string value, string? description = null, bool isEnabled = true)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Error.Validation("SystemSetting.Key.Required", "Key is required");

        if (key.Length > 100)
            return Error.Validation("SystemSetting.Key.TooLong", "Key cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("SystemSetting.Value.Required", "Value is required");

        if (value.Length > 500)
            return Error.Validation("SystemSetting.Value.TooLong", "Value cannot exceed 500 characters");

        if (description != null && description.Length > 200)
            return Error.Validation("SystemSetting.Description.TooLong", "Description cannot exceed 200 characters");

        return new SystemSetting(key.Trim(), value.Trim(), description?.Trim(), isEnabled);
    }

    public void UpdateValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required", nameof(value));

        if (value.Length > 500)
            throw new ArgumentException("Value cannot exceed 500 characters", nameof(value));

        Value = value.Trim();
    }

    public void Toggle()
    {
        IsEnabled = !IsEnabled;
    }

    public void Enable()
    {
        IsEnabled = true;
    }

    public void Disable()
    {
        IsEnabled = false;
    }
}
