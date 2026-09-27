namespace JadaraITKnowledgeSystem.API.Contracts;

public sealed record UpdateUniversityRequest(string Name);
public sealed record UpdateFacultyRequest(string Name, int UniversityId);
public sealed record UpdateMajorRequest(string Name, int FacultyId);
