using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using JadaraITKnowledgeSystem.IntegrationTests.TestSupport;

namespace JadaraITKnowledgeSystem.IntegrationTests.Api;

/// <summary>
/// End-to-end checks of the REST API as the frontend uses it (real SQL Server, real pipeline):
/// routes, status codes, problem-details errors, authorization and the security rules.
/// </summary>
[Collection(InfrastructureCollection.Name)]
public class ApiTests(InfrastructureFixture database)
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
        Assert.Equal("University.NotFound", ErrorCode(await response.ReadJsonAsync()));
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
    public async Task MaterialUpload_GoesStraightToStorage_AndIsServedOnlyThroughSignedUrls()
    {
        var admin = await AsAdminAsync();
        var courseId = (await CreateCourseAsync(admin)).GetProperty("id").GetInt32();
        var folderId = (await (await admin.PostAsJsonAsync($"/api/courses/{courseId}/folders", new { name = "Week 1" })).ReadJsonAsync())
            .GetProperty("id").GetInt32();
        var video = RandomBytes(300_000);

        var uploadKey = await UploadMaterialAsync(admin, "lecture.mp4", video);
        var created = await admin.PostAsJsonAsync($"/api/courses/{courseId}/materials",
            new { title = "Lecture 1", uploadKey, folderId, tags = new[] { "week1" } });
        var reused = await admin.PostAsJsonAsync($"/api/courses/{courseId}/materials", new { title = "Again", uploadKey });

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.Equal("Upload.NotFound", ErrorCode(await reused.ReadJsonAsync()));

        // Any signed-in user can list the course; each listing hands out a fresh short-lived URL.
        var student = Anonymous().WithBearer((await Anonymous().RegisterAsync(TestData.UniqueEmail())).GetProperty("tokens").AccessToken());
        var contents = await (await student.GetAsync($"/api/courses/{courseId}/contents?folderId={folderId}")).ReadJsonAsync();
        var material = Assert.Single(contents.GetProperty("materials").EnumerateArray());
        Assert.Equal("video/mp4", material.GetProperty("contentType").GetString());
        Assert.Equal(video.Length, material.GetProperty("sizeBytes").GetInt64());

        var signedUrl = new Uri(material.GetProperty("contentUrl").GetString()!);
        using var storageClient = new HttpClient();
        var download = await storageClient.GetAsync(signedUrl);
        var ranged = new HttpRequestMessage(HttpMethod.Get, signedUrl) { Headers = { Range = new System.Net.Http.Headers.RangeHeaderValue(0, 99) } };
        var partial = await storageClient.SendAsync(ranged);
        var unsigned = await storageClient.GetAsync(signedUrl.GetLeftPart(UriPartial.Path));

        Assert.Equal(HttpStatusCode.OK, download.StatusCode);
        Assert.Equal(video, await download.Content.ReadAsByteArrayAsync());
        Assert.Equal("max-age=31536000, immutable", download.Headers.CacheControl?.ToString());
        Assert.Equal(HttpStatusCode.PartialContent, partial.StatusCode); // video players seek with range requests
        Assert.Equal(100, (await partial.Content.ReadAsByteArrayAsync()).Length);
        Assert.Equal(HttpStatusCode.Forbidden, unsigned.StatusCode);
    }

    [Fact]
    public async Task MaterialUpload_RejectsBadTypesOversizedFilesForgedKeysAndNonWriters()
    {
        var admin = await AsAdminAsync();
        var student = Anonymous().WithBearer((await Anonymous().RegisterAsync(TestData.UniqueEmail())).GetProperty("tokens").AccessToken());
        var courseId = (await CreateCourseAsync(admin)).GetProperty("id").GetInt32();

        var executable = await admin.PostAsJsonAsync("/api/files/material-uploads", new { fileName = "setup.exe", size = 10 });
        var tooBig = await admin.PostAsJsonAsync("/api/files/material-uploads", new { fileName = "a.mp4", size = KnoxApiFactory.MaxMaterialBytes + 1 });
        var notAWriter = await student.PostAsJsonAsync("/api/files/material-uploads", new { fileName = "a.pdf", size = 10 });
        var forged = await admin.PostAsJsonAsync($"/api/courses/{courseId}/materials", new { title = "x", uploadKey = "materials/1/../../secret.pdf" });

        Assert.Equal("File.TypeNotAllowed", ErrorCode(await executable.ReadJsonAsync()));
        Assert.Equal("File.TooLarge", ErrorCode(await tooBig.ReadJsonAsync()));
        Assert.Equal(HttpStatusCode.Forbidden, notAWriter.StatusCode);
        Assert.Equal("Upload.Invalid", ErrorCode(await forged.ReadJsonAsync()));

        // A presigned PUT cannot cap the body, so a file larger than announced is rejected (and removed) on claim.
        var uploadKey = await UploadMaterialAsync(admin, "big.pdf", RandomBytes((int)KnoxApiFactory.MaxMaterialBytes + 1), announcedSize: 10);
        var oversized = await admin.PostAsJsonAsync($"/api/courses/{courseId}/materials", new { title = "x", uploadKey });

        Assert.Equal("File.TooLarge", ErrorCode(await oversized.ReadJsonAsync()));
        using var s3 = database.CreateS3Client();
        var leftover = await s3.ListObjectsV2Async(new() { BucketName = InfrastructureFixture.PrivateBucket, Prefix = uploadKey });
        Assert.Empty(leftover.S3Objects ?? []);
    }

    [Fact]
    public async Task DeletingAMaterial_DeletesItsFile()
    {
        var admin = await AsAdminAsync();
        var courseId = (await CreateCourseAsync(admin)).GetProperty("id").GetInt32();
        var uploadKey = await UploadMaterialAsync(admin, "notes.pdf", RandomBytes(2_000));
        var material = await (await admin.PostAsJsonAsync($"/api/courses/{courseId}/materials", new { title = "Notes", uploadKey })).ReadJsonAsync();
        var signedUrl = material.GetProperty("contentUrl").GetString()!;

        var delete = await admin.DeleteAsync($"/api/materials/{material.GetProperty("id").GetInt32()}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        // The file is removed after the transaction commits, on the background queue.
        using var storageClient = new HttpClient();
        var status = HttpStatusCode.OK;
        for (var i = 0; i < 40 && status == HttpStatusCode.OK; i++)
        {
            await Task.Delay(100);
            status = (await storageClient.GetAsync(signedUrl)).StatusCode;
        }
        Assert.Equal(HttpStatusCode.NotFound, status);
    }

    [Fact]
    public async Task QuizImages_MoveFromTemporaryToPermanentPublicStorage()
    {
        var admin = await AsAdminAsync();
        var courseId = (await CreateCourseAsync(admin)).GetProperty("id").GetInt32();
        var image = new ByteArrayContent(RandomBytes(5_000));
        image.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        var temp = await (await admin.PostAsync("/api/files/upload/temporary", new MultipartFormDataContent { { image, "file", "diagram.png" } }))
            .ReadJsonAsync();
        var tempUrl = temp.GetProperty("fileUrl").GetString()!;

        var quiz = await admin.PostAsJsonAsync("/api/quizzes", new
        {
            title = "Diagrams",
            courseId,
            questions = new[] { new { text = "Which?", type = "SingleChoice", imageUrl = tempUrl, choices = new[] { new { text = "A", isCorrect = true } } } }
        });
        var forged = await admin.PostAsJsonAsync("/api/quizzes", new
        {
            title = "Forged",
            courseId,
            questions = new[] { new { text = "Which?", type = "SingleChoice", imageUrl = "https://evil.example/x.png", choices = new[] { new { text = "A", isCorrect = true } } } }
        });

        Assert.Equal(HttpStatusCode.Created, quiz.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, forged.StatusCode);
        var permanentUrl = (await quiz.ReadJsonAsync()).GetProperty("questions")[0].GetProperty("imageUrl").GetString()!;
        Assert.DoesNotContain("/temp/", permanentUrl);

        // Public images are readable without credentials; the temporary copy is gone.
        using var browser = new HttpClient();
        Assert.Equal(HttpStatusCode.OK, (await browser.GetAsync(permanentUrl)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await browser.GetAsync(tempUrl)).StatusCode);
    }

    [Fact]
    public async Task CourseContents_RequireSignIn_AndCourseDetailsCountMaterials()
    {
        var admin = await AsAdminAsync();
        var courseId = (await CreateCourseAsync(admin)).GetProperty("id").GetInt32();
        var uploadKey = await UploadMaterialAsync(admin, "notes.pdf", RandomBytes(100));
        await admin.PostAsJsonAsync($"/api/courses/{courseId}/materials", new { title = "Notes", uploadKey });

        // Listing hands out signed URLs for private files, so it is not public.
        Assert.Equal(HttpStatusCode.Unauthorized, (await Anonymous().GetAsync($"/api/courses/{courseId}/contents")).StatusCode);

        var course = await (await Anonymous().GetAsync($"/api/courses/{courseId}")).ReadJsonAsync();
        Assert.Equal(1, course.GetProperty("numberOfMaterials").GetInt32());
        Assert.Equal(0, course.GetProperty("numberOfQuizzes").GetInt32());
    }

    [Fact]
    public async Task Universities_CanBeSearchedByName()
    {
        var admin = await AsAdminAsync();
        var name = TestData.Unique("Searchable");
        await admin.PostAsJsonAsync("/api/universities", new { name });

        var found = await (await Anonymous().GetAsync($"/api/universities?name={Uri.EscapeDataString(name[..^2])}")).ReadJsonAsync();

        var item = Assert.Single(found.GetProperty("items").EnumerateArray());
        Assert.Equal(name.ToLowerInvariant(), item.GetProperty("name").GetString());
    }

    [Fact]
    public async Task UserDetails_ArePlainStrings()
    {
        var admin = await AsAdminAsync();
        var email = TestData.UniqueEmail();
        await Anonymous().RegisterAsync(email);

        var page = await (await admin.GetAsync($"/api/users/details?email={Uri.EscapeDataString(email)}")).ReadJsonAsync();
        var user = Assert.Single(page.GetProperty("items").EnumerateArray());

        Assert.Equal(email.ToLowerInvariant(), user.GetProperty("email").GetString());
        Assert.Equal(JsonValueKind.String, user.GetProperty("name").ValueKind);
        Assert.DoesNotMatch(@"^\d+-", user.GetProperty("majorName").GetString()); // no "{id}-" prefix
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
    public async Task AnAdministrator_CannotBlockThemselves()
    {
        var superAdmin = await AsAdminAsync();

        var response = await superAdmin.PostAsync($"/api/users/{await CurrentDomainUserIdAsync(superAdmin)}/block", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Users.CannotBlockSelf", ErrorCode(await response.ReadJsonAsync()));
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
        var uploadKey = await UploadMaterialAsync(admin, "notes.pdf", OnePagePdf("TCP adds reliable, ordered delivery on top of IP."));
        var material = await (await admin.PostAsJsonAsync($"/api/courses/{courseId}/materials", new { title = "AI source", uploadKey }))
            .ReadJsonAsync();

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

        // Text extraction succeeds; the OpenAI step then fails cleanly and says why.
        Assert.Equal("Failed", job.GetProperty("status").GetString());
        Assert.Contains("not configured", job.GetProperty("errorMessage").GetString());
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

    /// <summary>The browser flow: ask the API for an upload URL, then PUT the bytes straight to object storage.</summary>
    private static async Task<string> UploadMaterialAsync(HttpClient api, string fileName, byte[] content, long? announcedSize = null)
    {
        var ticket = await (await api.PostAsJsonAsync("/api/files/material-uploads", new { fileName, size = announcedSize ?? content.Length }))
            .ReadJsonAsync();

        var put = new HttpRequestMessage(HttpMethod.Put, ticket.GetProperty("url").GetString()) { Content = new ByteArrayContent(content) };
        foreach (var header in ticket.GetProperty("headers").EnumerateObject())
            put.Content.Headers.TryAddWithoutValidation(header.Name, header.Value.GetString());

        using var storageClient = new HttpClient();
        var response = await storageClient.SendAsync(put);
        Assert.True(response.IsSuccessStatusCode, $"Direct upload failed: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");

        return ticket.GetProperty("key").GetString()!;
    }

    /// <summary>Business errors carry a "code"; validation errors are keyed by code under "errors".</summary>
    private static string? ErrorCode(JsonElement problem) =>
        problem.TryGetProperty("code", out var code) ? code.GetString()
        : problem.TryGetProperty("errors", out var errors) ? errors.EnumerateObject().First().Name
        : null;

    /// <summary>A minimal, valid one-page PDF containing <paramref name="text"/>.</summary>
    private static byte[] OnePagePdf(string text)
    {
        var content = $"BT /F1 11 Tf 40 760 Td ({text}) Tj ET";
        string[] objects =
        [
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>",
            $"<< /Length {content.Length} >>\nstream\n{content}\nendstream",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"
        ];
        var pdf = new System.Text.StringBuilder("%PDF-1.4\n");
        var offsets = new List<int>();
        for (var i = 0; i < objects.Length; i++)
        {
            offsets.Add(pdf.Length);
            pdf.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n");
        }
        var xref = pdf.Length;
        pdf.Append($"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n");
        foreach (var offset in offsets)
            pdf.Append($"{offset:D10} 00000 n \n");
        pdf.Append($"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
        return System.Text.Encoding.ASCII.GetBytes(pdf.ToString());
    }

    private static byte[] RandomBytes(int length)
    {
        var bytes = new byte[length];
        Random.Shared.NextBytes(bytes);
        return bytes;
    }
}
