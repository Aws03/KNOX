namespace JadaraITKnowledgeSystem.Infrastructure.Services.Security;

public static class CustomClaimTypes
{
    /// <summary>Carries the domain <c>Users.Id</c>, which differs from the identity id in the JWT subject.</summary>
    public const string DomainUserId = "domain_user_id";
}
