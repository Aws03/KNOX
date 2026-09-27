namespace JadaraITKnowledgeSystem.API.Contracts;

public sealed record ToggleFeatureRequest(bool Enabled);
public sealed record FeatureStatusResponse(bool Enabled);
