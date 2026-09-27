using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Infrastructure.Services.AI;
using JadaraITKnowledgeSystem.Infrastructure.Services.JWT;
using JadaraITKnowledgeSystem.Infrastructure.Services.Security;
using JadaraITKnowledgeSystem.Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Http;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using MsOptions = Microsoft.Extensions.Options.Options;

namespace JadaraITKnowledgeSystem.UnitTests.Infrastructure;

public sealed class LocalFileStorageTests : IDisposable
{
    private readonly string _contentRoot = Directory.CreateTempSubdirectory("knox-storage-").FullName;
    private readonly LocalFileStorage _storage;

    public LocalFileStorageTests()
    {
        var env = Substitute.For<IHostEnvironment>();
        env.ContentRootPath.Returns(_contentRoot);
        _storage = new LocalFileStorage(env, MsOptions.Create(new StorageOptions { BaseUrl = "http://files.test/" }));
    }

    public void Dispose() => Directory.Delete(_contentRoot, recursive: true);

    [Fact]
    public async Task Upload_ThenDownload_RoundTripsInsideTheUploadsFolder()
    {
        var url = await _storage.UploadAsync(new MemoryStream("hello"u8.ToArray()), "a.txt", "permanent/material");

        await using var downloaded = await _storage.DownloadAsync("a.txt", "permanent/material");

        Assert.Equal("http://files.test/uploads/permanent/material/a.txt", url);
        Assert.Equal("hello", await new StreamReader(downloaded!).ReadToEndAsync());
        Assert.True(File.Exists(Path.Combine(_contentRoot, "wwwroot", "uploads", "permanent", "material", "a.txt")));
    }

    [Theory]
    [InlineData("permanent/../../../escape")]
    [InlineData("../outside")]
    [InlineData("/etc")]
    public async Task Upload_WithAFolderOutsideTheUploadsRoot_IsRejected(string folder)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _storage.UploadAsync(new MemoryStream([1]), "x.pdf", folder));
    }

    [Theory]
    [InlineData("../x.pdf")]
    [InlineData("sub/x.pdf")]
    [InlineData("..")]
    public async Task Delete_WithAFileNameContainingPathSegments_IsRejected(string fileName)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _storage.DeleteAsync(fileName, "temp"));
    }
}

public class SecurityServiceTests
{
    private static JwtTokenService JwtService() => new(
        MsOptions.Create(new JwtOptions
        {
            Secret = "unit-test-secret-that-is-long-enough-for-hs256",
            Issuer = "issuer",
            Audience = "audience",
            ExpirationMinutes = 15
        }),
        TimeProvider.System);

    [Fact]
    public async Task JwtToken_CarriesIdentityIdDomainIdAndASingleRole()
    {
        var token = await JwtService()
            .GenerateJwtTokenAsync(userId: 5, domainUserId: 9, "Name", "a@b.co", ["Writer", "User"]);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("5", jwt.Subject);
        Assert.Equal("9", jwt.Claims.Single(c => c.Type == CustomClaimTypes.DomainUserId).Value);
        Assert.Equal("Writer", Assert.Single(jwt.Claims, c => c.Type == "role").Value);
    }

    [Fact]
    public void CurrentUser_ReadsDomainIdFromItsOwnClaim()
    {
        var service = CurrentUserFor(new Claim(ClaimTypes.NameIdentifier, "5"), new Claim(CustomClaimTypes.DomainUserId, "9"));

        Assert.Equal(5, service.UserId);
        Assert.Equal(9, service.DomainUserId);
    }

    [Fact]
    public void CurrentUser_TokenWithoutDomainClaim_FallsBackToTheIdentityId()
    {
        var service = CurrentUserFor(new Claim(ClaimTypes.NameIdentifier, "5"));

        Assert.Equal(5, service.DomainUserId);
    }

    [Fact]
    public void CurrentUser_Anonymous_HasNoIdentity()
    {
        var service = new CurrentUserService(new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

        Assert.Null(service.UserId);
        Assert.Null(service.DomainUserId);
        Assert.Empty(service.Roles);
    }

    private static CurrentUserService CurrentUserFor(params Claim[] claims) =>
        new(new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test")) }
        });
}

public class OpenAIServiceTests
{
    [Fact]
    public async Task GenerateQuiz_WithoutAnApiKey_FailsWithoutCallingOpenAI()
    {
        var service = new OpenAIService(
            new HttpClient(new StubHandler("unused")), MsOptions.Create(new OpenAIOptions()), NullLogger<OpenAIService>.Instance);

        var result = await service.GenerateQuizFromTextAsync(new GenerateQuizRequest { Text = "x" });

        Assert.Equal("OpenAI.NotConfigured", result.TopError.Code);
    }

    private sealed class StubHandler(string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    [Fact]
    public async Task GenerateQuiz_KeepsOnlyWellFormedQuestions_AsSingleChoice()
    {
        const string quizJson = """
            {"topic":"Sorting","questions":[
              {"text":"Fastest average sort?","type":0,"choices":[{"text":"Quick","isCorrect":true},{"text":"Bubble","isCorrect":false},{"text":"Insertion","isCorrect":false},{"text":"Selection","isCorrect":false}]},
              {"text":"Two correct answers","type":1,"choices":[{"text":"A","isCorrect":true},{"text":"B","isCorrect":true},{"text":"C","isCorrect":false},{"text":"D","isCorrect":false}]}
            ]}
            """;
        var completion = System.Text.Json.JsonSerializer.Serialize(new
        {
            choices = new[] { new { message = new { content = "```json\n" + quizJson + "\n```" } } }
        });
        var service = new OpenAIService(
            new HttpClient(new StubHandler(completion)),
            MsOptions.Create(new OpenAIOptions { ApiKey = "test" }),
            NullLogger<OpenAIService>.Instance);

        var result = await service.GenerateQuizFromTextAsync(new GenerateQuizRequest { Text = "sorting algorithms" });

        var question = Assert.Single(result.Value.Questions);
        Assert.Equal("Fastest average sort?", question.Text);
        Assert.Equal(QuestionType.SingleChoice, question.Type);
        Assert.Equal("Sorting", result.Value.Topic);
        Assert.Equal("Test your knowledge with this quiz.", result.Value.Description);
    }
}
