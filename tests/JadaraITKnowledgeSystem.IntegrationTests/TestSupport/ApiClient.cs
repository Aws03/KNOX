using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace JadaraITKnowledgeSystem.IntegrationTests.TestSupport;

public static class ApiClient
{
    public const string AdminEmail = "admin@knox.com";
    public const string AdminPassword = "Admin@123456";
    public const string DefaultPassword = "Student1Pass";

    public static async Task<JsonElement> ReadJsonAsync(this HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(string.IsNullOrEmpty(body) ? "null" : body).RootElement.Clone();
    }

    public static async Task<JsonElement> LoginAsync(this HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        var body = await response.ReadJsonAsync();
        Assert.True(response.IsSuccessStatusCode, $"Login failed: {(int)response.StatusCode} {body}");
        return body;
    }

    public static HttpClient WithBearer(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    public static string AccessToken(this JsonElement tokens) => tokens.GetProperty("accessToken").GetString()!;

    /// <summary>Walks the hierarchy DataSeeder creates (other tests may add more universities).</summary>
    public static async Task<int> GetSeededMajorIdAsync(this HttpClient client)
    {
        var universityId = await FindIdByNameAsync(client, "/api/universities", "jadara university");
        var facultyId = await FindIdByNameAsync(client, $"/api/faculties/by-university/{universityId}", "Faculty of Information Technology");
        return await FindIdByNameAsync(client, $"/api/majors/by-faculty/{facultyId}", "Computer Science");
    }

    private static async Task<int> FindIdByNameAsync(HttpClient client, string url, string name)
    {
        var page = await (await client.GetAsync($"{url}?pageNumber=1&pageSize=50")).ReadJsonAsync();
        return page.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("name").GetString() == name)
            .GetProperty("id").GetInt32();
    }

    /// <summary>Registers a user (verification is off) and returns { user, requiresVerification, otpSent, tokens }.</summary>
    public static async Task<JsonElement> RegisterAsync(this HttpClient client, string email, string password = DefaultPassword)
    {
        var majorId = await client.GetSeededMajorIdAsync();
        var response = await client.PostAsJsonAsync("/api/auth/register",
            new { email, password, fullName = "Integration Student", majorId });
        var body = await response.ReadJsonAsync();
        Assert.True(response.IsSuccessStatusCode, $"Register failed: {(int)response.StatusCode} {body}");
        return body;
    }

    public static int DomainUserId(this JsonElement registration) =>
        registration.GetProperty("user").GetProperty("domainUserId").GetInt32();
}
