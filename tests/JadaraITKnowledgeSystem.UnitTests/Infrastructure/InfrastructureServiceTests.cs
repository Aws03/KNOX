using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Infrastructure.Services.AI;
using JadaraITKnowledgeSystem.Infrastructure.Services.JWT;
using JadaraITKnowledgeSystem.Infrastructure.Services.Security;
using JadaraITKnowledgeSystem.Infrastructure.Services.FileManagement;
using JadaraITKnowledgeSystem.Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Http;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using MsOptions = Microsoft.Extensions.Options.Options;

namespace JadaraITKnowledgeSystem.UnitTests.Infrastructure;

public class StorageTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 27, 12, 0, 0, TimeSpan.Zero);

    private static StorageOptions Options(Action<StorageOptions>? configure = null)
    {
        var options = new StorageOptions
        {
            ServiceUrl = "http://s3.internal:8333",
            PublicServiceUrl = "https://files.example.com",
            AccessKey = "key",
            SecretKey = "secret",
            PublicBucket = "knox-public",
            PrivateBucket = "knox-private",
            PublicBaseUrl = "https://cdn.example.com/",
            MaxMaterialBytes = 1_000
        };
        configure?.Invoke(options);
        return options;
    }

    [Fact]
    public void BunnyToken_MatchesTheDocumentedAlgorithm()
    {
        var expires = DateTimeOffset.FromUnixTimeSeconds(1_800_000_000);

        var url = BunnyTokenSigner.Sign("https://knox.b-cdn.net/", "materials/1/a b.mp4", "secret-key", expires);

        // base64url(sha256("secret-key" + "/materials/1/a%20b.mp4" + "1800000000")), padding removed.
        var expected = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            Encoding.UTF8.GetBytes("secret-key/materials/1/a%20b.mp41800000000"))).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        Assert.Equal($"https://knox.b-cdn.net/materials/1/a%20b.mp4?token={expected}&expires=1800000000", url.AbsoluteUri);
    }

    [Fact]
    public void PrivateDownloads_UseTheCdnWhenConfigured_OtherwiseAPresignedUrl()
    {
        using var viaCdn = new S3StorageService(MsOptions.Create(Options(o => { o.CdnBaseUrl = "https://knox.b-cdn.net"; o.CdnTokenKey = "k"; })));
        using var direct = new S3StorageService(MsOptions.Create(Options()));

        var cdnUrl = viaCdn.CreateDownloadUrl("materials/1/a.mp4", Now.AddHours(1));
        var presigned = direct.CreateDownloadUrl("materials/1/a.mp4", Now.AddHours(1));

        Assert.StartsWith("https://knox.b-cdn.net/materials/1/a.mp4?token=", cdnUrl.AbsoluteUri);
        // Presigned URLs are signed for the browser-facing endpoint, never the internal one.
        Assert.StartsWith("https://files.example.com/knox-private/materials/1/a.mp4?", presigned.AbsoluteUri);
        Assert.Contains("X-Amz-Signature=", presigned.Query);
    }

    [Theory]
    [InlineData("https://cdn.example.com/profile-pictures/1/a.png", "profile-pictures/1/a.png")]
    [InlineData("https://cdn.example.com/../knox-private/x.pdf", null)]
    [InlineData("https://elsewhere.example.com/profile-pictures/1/a.png", null)]
    [InlineData("https://cdn.example.com/a b.png", null)]
    public void PublicKeys_AreOnlyRecognisedForThisStoragesUrls(string url, string? expected)
    {
        using var storage = new S3StorageService(MsOptions.Create(Options()));

        Assert.Equal(expected, storage.GetPublicKey(url));
    }

    [Theory]
    [InlineData("../escape.pdf")]
    [InlineData("/absolute.pdf")]
    [InlineData("materials/../../x")]
    public void Keys_WithPathTricks_AreRejected(string key)
    {
        using var storage = new S3StorageService(MsOptions.Create(Options()));

        Assert.Throws<ArgumentException>(() => storage.GetPublicUrl(key));
    }

    [Theory]
    [InlineData("movie.exe", 10, "File.TypeNotAllowed")]
    [InlineData("movie.mp4", 0, "File.Empty")]
    [InlineData("movie.mp4", 1_001, "File.TooLarge")]
    public void MaterialUploads_AreValidatedBeforeAnythingIsSigned(string fileName, long size, string error)
    {
        var storage = Substitute.For<IStorageService>();
        var files = new FileManager(storage, MsOptions.Create(Options()), TimeProvider.System, NullLogger<FileManager>.Instance);

        Assert.Equal(error, files.CreateMaterialUpload(fileName, size).TopError.Code);
        storage.DidNotReceiveWithAnyArgs().CreateUploadUrl(default, default!, default!, default);
    }

    [Fact]
    public void MaterialUploads_AreSignedForTheExactContentTypeUnderTemp()
    {
        var storage = Substitute.For<IStorageService>();
        storage.CreateUploadUrl(default, default!, default!, default).ReturnsForAnyArgs(new Uri("https://files.example.com/put"));
        var files = new FileManager(storage, MsOptions.Create(Options()), TimeProvider.System, NullLogger<FileManager>.Instance);

        var upload = files.CreateMaterialUpload("Lecture 1.MP4", 500).Value;

        Assert.Matches("^temp/materials/[0-9a-f]{32}\\.mp4$", upload.Key);
        Assert.Equal("video/mp4", upload.Headers["Content-Type"]);
        storage.Received(1).CreateUploadUrl(StorageBucket.Private, upload.Key, "video/mp4", Arg.Any<DateTimeOffset>());
    }

    [Fact]
    public void Options_RequireCdnSettingsAndCredentialsInPairs()
    {
        var options = Options(o => { o.CdnBaseUrl = "https://knox.b-cdn.net"; o.SecretKey = null; });

        var errors = options.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(options)).ToList();

        Assert.Equal(2, errors.Count);
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
