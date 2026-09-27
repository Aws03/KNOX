using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using JadaraITKnowledgeSystem.IntegrationTests.TestSupport;

namespace JadaraITKnowledgeSystem.IntegrationTests.Api;

/// <summary>
/// End-to-end checks of the REST API as the frontend uses it (real SQL Server, real pipeline):
/// routes, status codes, problem-details errors, authorization and the security rules.
/// </summary>
[Collection(SqlServerCollection.Name)]
public class ApiTests(SqlServerFixture database)
{
    private HttpClient Anonymous() => database.Api.CreateClient();

    private async Task<HttpClient> AsAdminAsync() =>
        Anonymous().WithBearer((await Anonymous().LoginAsync(ApiClient.AdminEmail, ApiClient.AdminPassword)).AccessToken());

    private async Task<int> CurrentDomainUserIdAsync(HttpClient client) =>
        (await (await client.GetAsync("/api/users/me")).ReadJsonAsync()).GetProperty("domainUserId").GetInt32();

    [Fact]
    public async Task OpenApiDocument_AndHealthEndpoints_AreServed()
    {
        var client = Anonymous();

        var openApi = await client.GetAsync("/openapi/v1.json");
        var live = await client.GetAsync("/health/live");
        var ready = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, openApi.StatusCode);
        Assert.Contains("\"Bearer\"", await openApi.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, live.StatusCode);
        Assert.Equal(HttpStatusCode.OK, ready.StatusCode);
    }

    [Fact]
    public async Task Responses_CarrySecurityHeaders()
    {
        var response = await Anonymous().GetAsync("/api/universities");

        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
    }

    [Fact]
    public async Task Universities_ArePublicAndIncludeTheSeededOne()
    {
        var response = await Anonymous().GetAsync("/api/universities?pageNumber=1&pageSize=10");
        var body = await response.ReadJsonAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(body.GetProperty("items").EnumerateArray(), u => u.GetProperty("name").GetString() == "jadara university");
        Assert.True(body.TryGetProperty("totalCount", out _));
    }

    [Fact]
    public async Task ValidationFailures_AreValidationProblemDetails()
    {
        var response = await Anonymous().GetAsync("/api/universities?pageNumber=1&pageSize=500");
        var problem = await response.ReadJsonAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(400, problem.GetProperty("status").GetInt32());
        Assert.Contains("Page size", problem.GetProperty("detail").GetString());
        Assert.True(problem.GetProperty("errors").TryGetProperty("PageSize", out _));
        Assert.True(problem.TryGetProperty("traceId", out _));
    }

    [Fact]
    public async Task NotFound_IsProblemDetailsWithAnErrorCode()
    {
        var response = await Anonymous().GetAsync("/api/courses/by-code/NO-SUCH-CODE");
        var problem = await response.ReadJsonAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Course.NotFound", problem.GetProperty("code").GetString());
        Assert.False(string.IsNullOrEmpty(problem.GetProperty("detail").GetString()));
    }

    [Fact]
    public async Task Login_ReturnsTokens_AndWrongPasswordIs401ProblemDetails()
    {
        var tokens = await Anonymous().LoginAsync(ApiClient.AdminEmail, ApiClient.AdminPassword);
        var wrong = await Anonymous().PostAsJsonAsync("/api/auth/login", new { email = ApiClient.AdminEmail, password = "wrong-password" });

        Assert.False(string.IsNullOrEmpty(tokens.GetProperty("refreshToken").GetString()));
        Assert.Equal(HttpStatusCode.Unauthorized, wrong.StatusCode);
        Assert.Equal("Invalid credentials", (await wrong.ReadJsonAsync()).GetProperty("detail").GetString());
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Is401()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, (await Anonymous().GetAsync("/api/users/me")).StatusCode);
    }

    [Fact]
    public async Task Me_ReturnsTheSeededSuperAdminProfile()
    {
        var response = await (await AsAdminAsync()).GetAsync("/api/users/me");
        var body = await response.ReadJsonAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("SuperAdmin", body.GetProperty("role").GetString());
        Assert.Equal("jadara university", body.GetProperty("universityName").GetString());
    }

    [Fact]
    public async Task RefreshToken_Rotates_AndReusingTheOldOneRevokesTheWholeSession()
    {
        var registration = await Anonymous().RegisterAsync(TestData.UniqueEmail());
        var firstRefresh = registration.GetProperty("tokens").GetProperty("refreshToken").GetString();

        var rotated = await Anonymous().PostAsJsonAsync("/api/auth/refresh", new { refreshToken = firstRefresh });
        var secondRefresh = (await rotated.ReadJsonAsync()).GetProperty("refreshToken").GetString();
        var reused = await Anonymous().PostAsJsonAsync("/api/auth/refresh", new { refreshToken = firstRefresh });
        var afterReuse = await Anonymous().PostAsJsonAsync("/api/auth/refresh", new { refreshToken = secondRefresh });

        Assert.Equal(HttpStatusCode.OK, rotated.StatusCode);
        Assert.NotEqual(firstRefresh, secondRefresh);
        Assert.Equal(HttpStatusCode.Unauthorized, reused.StatusCode);
        // Reuse of a rotated token means it leaked: every token of that user is revoked.
        Assert.Equal(HttpStatusCode.Unauthorized, afterReuse.StatusCode);
    }

    [Fact]
    public async Task Register_SignsTheNewUserInWithTheUserRole()
    {
        var body = await Anonymous().RegisterAsync(TestData.UniqueEmail());

        Assert.False(body.GetProperty("requiresVerification").GetBoolean());
        Assert.Equal("User", body.GetProperty("user").GetProperty("assignedRole").GetString());

        var me = await Anonymous().WithBearer(body.GetProperty("tokens").AccessToken()).GetAsync("/api/users/me");
        Assert.Equal("User", (await me.ReadJsonAsync()).GetProperty("role").GetString());
    }

    [Fact]
    public async Task Register_WithAWeakPassword_ListsTheProblems()
    {
        var majorId = await Anonymous().GetSeededMajorIdAsync();

        var response = await Anonymous().PostAsJsonAsync("/api/auth/register",
            new { email = TestData.UniqueEmail(), password = "short", fullName = "Weak", majorId });
        var problem = await response.ReadJsonAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(problem.GetProperty("errors").TryGetProperty("Password", out _));
    }

    [Fact]
    public async Task PasswordReset_WorksWithTheEmailedOtp_AndOtpsAreBurnedAfterFiveWrongGuesses()
    {
        var email = TestData.UniqueEmail();
        await Anonymous().RegisterAsync(email);

        var send = await Anonymous().PostAsJsonAsync("/api/auth/send-verification-otp", new { email });
        Assert.Equal(HttpStatusCode.NoContent, send.StatusCode);

        // No email provider in tests: read the code the way a mailbox would, from the database.
        string otp;
        await using (var context = database.CreateContext(connectionString: database.ConnectionStringFor("knox_api")))
        {
            var userId = context.Users.Single(u => u.Email.Address == email.ToUpperInvariant()).Id;
            otp = context.VerificationOTPs.Where(o => o.UserId == userId && !o.IsUsed).Select(o => o.OTP).Single();
        }

        var wrongCode = otp == "000000" ? "111111" : "000000";
        var wrong = await Anonymous().PostAsJsonAsync("/api/auth/reset-password", new { email, otp = wrongCode, newPassword = "NewPassw0rd" });
        var reset = await Anonymous().PostAsJsonAsync("/api/auth/reset-password", new { email, otp, newPassword = "NewPassw0rd" });
        var loginWithNew = await Anonymous().PostAsJsonAsync("/api/auth/login", new { email, password = "NewPassw0rd" });

        Assert.Equal(HttpStatusCode.BadRequest, wrong.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, reset.StatusCode);
        Assert.Equal(HttpStatusCode.OK, loginWithNew.StatusCode);
    }

    [Fact]
    public async Task FacultiesByUniversity_OnlyReturnsThatUniversitysFaculties()
    {
        var admin = await AsAdminAsync();
        var universityName = TestData.Unique("Filter University");
        var university = await (await admin.PostAsJsonAsync("/api/universities", new { name = universityName })).ReadJsonAsync();
        var universityId = university.GetProperty("id").GetInt32();
        var created = await admin.PostAsJsonAsync("/api/faculties", new { name = "Faculty of Filtering", universityId });

        var faculties = await (await Anonymous().GetAsync($"/api/faculties/by-university/{universityId}")).ReadJsonAsync();

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.Equal("Faculty of Filtering", Assert.Single(faculties.GetProperty("items").EnumerateArray()).GetProperty("name").GetString());
    }

    [Fact]
    public async Task CreateFaculty_ForAMissingUniversity_Is404()
    {
        var response = await (await AsAdminAsync()).PostAsJsonAsync("/api/faculties", new { name = "Orphan", universityId = 999_999 });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("University.NotFound", (await response.ReadJsonAsync()).GetProperty("code").GetString());
    }

    [Fact]
    public async Task Quiz_IsAttributedToTheCaller_AndReactionsAndAttemptsWork()
    {
        var admin = await AsAdminAsync();
        var adminDomainId = await CurrentDomainUserIdAsync(admin);
        var course = await CreateCourseAsync(admin);

        var quizResponse = await admin.PostAsJsonAsync("/api/quizzes", new
        {
            title = "Attribution quiz",
            writerId = 99999, // ignored: no longer part of the contract
            courseId = course.GetProperty("id").GetInt32(),
            questions = new[]
            {
                new
                {
                    text = "2 + 2 = ?",
                    type = 1, // numbers are still accepted for enums
                    choices = new[] { new { text = "4", isCorrect = true }, new { text = "5", isCorrect = false } }
                }
            },
            tags = new[] { "math" }
        });
        var quiz = await quizResponse.ReadJsonAsync();
        var quizId = quiz.GetProperty("id").GetInt32();

        Assert.Equal(HttpStatusCode.Created, quizResponse.StatusCode);
        Assert.Equal(adminDomainId, quiz.GetProperty("writerId").GetInt32());
        Assert.Equal("SingleChoice", quiz.GetProperty("questions")[0].GetProperty("type").GetString());
        Assert.EndsWith("+00:00", quiz.GetProperty("createdAt").GetString());

        var like = await admin.PostAsJsonAsync($"/api/quizzes/{quizId}/reactions", new { reactionType = 1 });
        var likeAgain = await admin.PostAsJsonAsync($"/api/quizzes/{quizId}/reactions", new { reactionType = "Like" });
        var dislike = await admin.PostAsJsonAsync($"/api/quizzes/{quizId}/reactions", new { reactionType = "Dislike" });
        var attempt = await admin.PostAsJsonAsync($"/api/quizzes/{quizId}/attempts", new { score = 80 });
        var retake = await admin.PostAsJsonAsync($"/api/quizzes/{quizId}/attempts", new { score = 95 });
        var list = await (await admin.GetAsync($"/api/quizzes/by-course/{course.GetProperty("id").GetInt32()}")).ReadJsonAsync();

        Assert.Equal(HttpStatusCode.OK, like.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, likeAgain.StatusCode);
        var counts = await dislike.ReadJsonAsync();
        Assert.Equal(0, counts.GetProperty("likes").GetInt32());
        Assert.Equal(1, counts.GetProperty("dislikes").GetInt32());
        Assert.Equal(HttpStatusCode.OK, attempt.StatusCode);
        Assert.Equal(HttpStatusCode.OK, retake.StatusCode);
        Assert.Equal(95, list.GetProperty("items")[0].GetProperty("lastAttemptScore").GetDecimal());
    }

    [Fact]
    public async Task ConcurrentReactions_FromDifferentUsers_AreAllCounted()
    {
        var admin = await AsAdminAsync();
        var course = await CreateCourseAsync(admin);
        var quiz = await (await admin.PostAsJsonAsync("/api/quizzes", new
        {
            title = "Popular quiz",
            courseId = course.GetProperty("id").GetInt32(),
            questions = new[] { new { text = "Q?", type = "SingleChoice", choices = new[] { new { text = "A", isCorrect = true } } } }
        })).ReadJsonAsync();
        var quizId = quiz.GetProperty("id").GetInt32();

        var users = await Task.WhenAll(Enumerable.Range(0, 4).Select(async _ =>
            Anonymous().WithBearer((await Anonymous().RegisterAsync(TestData.UniqueEmail())).GetProperty("tokens").AccessToken())));

        var responses = await Task.WhenAll(users.Select(u => u.PostAsJsonAsync($"/api/quizzes/{quizId}/reactions", new { reactionType = "Like" })));

        // Losers of the optimistic-concurrency race get 409 and may retry; nothing is silently lost.
        foreach (var (client, response) in users.Zip(responses))
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
                Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync($"/api/quizzes/{quizId}/reactions", new { reactionType = "Like" })).StatusCode);
            else
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        var final = await (await Anonymous().GetAsync($"/api/quizzes/{quizId}")).ReadJsonAsync();
        Assert.Equal(4, final.GetProperty("likes").GetInt32());
    }

    [Fact]
    public async Task PermanentUpload_RejectsPathTraversalAndNonWriters()
    {
        var admin = await AsAdminAsync();
        var student = Anonymous().WithBearer((await Anonymous().RegisterAsync(TestData.UniqueEmail())).GetProperty("tokens").AccessToken());

        var traversal = await admin.PostAsync("/api/files/upload/permanent", UploadForm("../../../outside"));
        var notAWriter = await student.PostAsync("/api/files/upload/permanent", UploadForm("material"));
        var legitimate = await admin.PostAsync("/api/files/upload/permanent", UploadForm("material"));

        Assert.Equal(HttpStatusCode.BadRequest, traversal.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, notAWriter.StatusCode);
        Assert.Equal(HttpStatusCode.OK, legitimate.StatusCode);
        var fileUrl = (await legitimate.ReadJsonAsync()).GetProperty("fileUrl").GetString()!;
        Assert.StartsWith("http://localhost/uploads/permanent/material/", fileUrl);

        var served = await Anonymous().GetAsync(new Uri(fileUrl).AbsolutePath);
        Assert.Equal(HttpStatusCode.OK, served.StatusCode);
    }

    [Fact]
    public async Task CourseMaterials_CanBeCreatedInFoldersAndListed()
    {
        var admin = await AsAdminAsync();
        var courseId = (await CreateCourseAsync(admin)).GetProperty("id").GetInt32();
        var folder = await (await admin.PostAsJsonAsync($"/api/courses/{courseId}/folders", new { name = "Week 1" })).ReadJsonAsync();
        var folderId = folder.GetProperty("id").GetInt32();

        var material = await admin.PostAsJsonAsync($"/api/courses/{courseId}/materials", new
        {
            title = "Lecture notes",
            contentUrl = "http://localhost/uploads/permanent/material/notes.pdf",
            folderId,
            tags = new[] { "week1" }
        });
        var contents = await (await Anonymous().GetAsync($"/api/courses/{courseId}/contents?folderId={folderId}")).ReadJsonAsync();

        Assert.Equal(HttpStatusCode.Created, material.StatusCode);
        Assert.Equal("Lecture notes", Assert.Single(contents.GetProperty("materials").EnumerateArray()).GetProperty("title").GetString());
    }

    [Fact]
    public async Task Enrollment_ListingAndCompletion_Work()
    {
        var admin = await AsAdminAsync();
        var courseId = (await CreateCourseAsync(admin)).GetProperty("id").GetInt32();
        var student = Anonymous().WithBearer((await Anonymous().RegisterAsync(TestData.UniqueEmail())).GetProperty("tokens").AccessToken());

        var enroll = await student.PostAsJsonAsync($"/api/courses/{courseId}/enroll", new { notes = (string?)null });
        var again = await student.PostAsJsonAsync($"/api/courses/{courseId}/enroll", new { notes = (string?)null });
        var complete = await student.PostAsync($"/api/courses/{courseId}/complete", null);
        var mine = await (await student.GetAsync("/api/courses/my-enrollments?isFinished=true")).ReadJsonAsync();

        Assert.Equal(HttpStatusCode.Created, enroll.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
        Assert.Equal(HttpStatusCode.OK, complete.StatusCode);
        Assert.Equal(courseId, Assert.Single(mine.GetProperty("items").EnumerateArray()).GetProperty("courseId").GetInt32());
    }

    [Fact]
    public async Task DashboardStatistics_AreAvailableToSuperAdmins()
    {
        var response = await (await AsAdminAsync()).GetAsync("/api/dashboard/statistics?months=3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, (await response.ReadJsonAsync()).GetProperty("usersGrowth").GetArrayLength());
    }

    [Fact]
    public async Task AssignRole_AnAdminCannotGrantSuperAdmin()
    {
        var superAdmin = await AsAdminAsync();
        var email = TestData.UniqueEmail();
        var promotedId = (await Anonymous().RegisterAsync(email)).DomainUserId();

        var makeAdmin = await superAdmin.PostAsJsonAsync($"/api/users/{promotedId}/assign-role", "Admin");
        Assert.Equal(HttpStatusCode.NoContent, makeAdmin.StatusCode);

        var admin = Anonymous().WithBearer((await Anonymous().LoginAsync(email, ApiClient.DefaultPassword)).AccessToken());
        var escalate = await admin.PostAsJsonAsync($"/api/users/{promotedId}/assign-role", "SuperAdmin");

        Assert.Equal(HttpStatusCode.Forbidden, escalate.StatusCode);
        Assert.Equal("Admin", (await (await admin.GetAsync("/api/users/me")).ReadJsonAsync()).GetProperty("role").GetString());
    }

    [Fact]
    public async Task BlockedUser_CanNeitherLogInNorRefresh()
    {
        var superAdmin = await AsAdminAsync();
        var email = TestData.UniqueEmail();
        var user = await Anonymous().RegisterAsync(email);

        var block = await superAdmin.PostAsync($"/api/users/{user.DomainUserId()}/block", null);
        var login = await Anonymous().PostAsJsonAsync("/api/auth/login", new { email, password = ApiClient.DefaultPassword });
        var refresh = await Anonymous().PostAsJsonAsync("/api/auth/refresh",
            new { refreshToken = user.GetProperty("tokens").GetProperty("refreshToken").GetString() });

        Assert.Equal(HttpStatusCode.NoContent, block.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
        Assert.Contains("blocked", (await login.ReadJsonAsync()).GetProperty("detail").GetString());
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }

    [Fact]
    public async Task WriterStatistics_AWriterCannotReadAnotherUsersStatistics()
    {
        var superAdmin = await AsAdminAsync();
        var email = TestData.UniqueEmail();
        var writerId = (await Anonymous().RegisterAsync(email)).DomainUserId();
        await superAdmin.PostAsJsonAsync($"/api/users/{writerId}/assign-role", "Writer");
        var writer = Anonymous().WithBearer((await Anonymous().LoginAsync(email, ApiClient.DefaultPassword)).AccessToken());

        var own = await writer.GetAsync($"/api/users/{writerId}/writer-statistics");
        var someoneElses = await writer.GetAsync($"/api/users/{await CurrentDomainUserIdAsync(superAdmin)}/writer-statistics");

        Assert.Equal(HttpStatusCode.OK, own.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, someoneElses.StatusCode);
    }

    [Fact]
    public async Task QuizGeneration_WithoutAnOpenAIKey_FailsTheJobCleanly()
    {
        var admin = await AsAdminAsync();
        var courseId = (await CreateCourseAsync(admin)).GetProperty("id").GetInt32();
        var upload = await (await admin.PostAsync("/api/files/upload/permanent", UploadForm("material"))).ReadJsonAsync();
        var material = await (await admin.PostAsJsonAsync($"/api/courses/{courseId}/materials", new
        {
            title = "AI source",
            contentUrl = upload.GetProperty("fileUrl").GetString()
        })).ReadJsonAsync();

        var accepted = await admin.PostAsJsonAsync($"/api/quiz-generation/materials/{material.GetProperty("id").GetInt32()}",
            new { questionsPerQuiz = 5, difficulty = "Easy", maxQuizzes = 1 });
        Assert.Equal(HttpStatusCode.Accepted, accepted.StatusCode);
        var jobUrl = accepted.Headers.Location!;

        // The job runs in the background after the request commits; poll until it settles.
        JsonElement job = default;
        for (var i = 0; i < 60; i++)
        {
            job = await (await admin.GetAsync(jobUrl)).ReadJsonAsync();
            if (job.GetProperty("status").GetString() is "Failed" or "Completed")
                break;
            await Task.Delay(250);
        }

        // The upload is not a real PDF, so extraction fails before any AI call is attempted.
        Assert.Equal("Failed", job.GetProperty("status").GetString());
        Assert.False(string.IsNullOrEmpty(job.GetProperty("errorMessage").GetString()));
    }

    private static async Task<JsonElement> CreateCourseAsync(HttpClient admin)
    {
        var majorId = await admin.GetSeededMajorIdAsync();
        var response = await admin.PostAsJsonAsync("/api/courses", new
        {
            majorId,
            courseName = TestData.Unique("Course"),
            courseCode = Guid.NewGuid().ToString("N")[..8],
            requirementType = "Major",
            requirementNature = "Compulsory",
            credits = 3
        });
        var body = await response.ReadJsonAsync();
        Assert.True(response.StatusCode == HttpStatusCode.Created, $"Create course failed: {(int)response.StatusCode} {body}");
        return body;
    }

    private static MultipartFormDataContent UploadForm(string category)
    {
        var file = new ByteArrayContent("%PDF-1.4 not really a pdf"u8.ToArray());
        file.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        return new MultipartFormDataContent
        {
            { file, "file", "notes.pdf" },
            { new StringContent(category), "fileCategory" }
        };
    }
}
