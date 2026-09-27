using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourse;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseById;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Queries.GetFacultiesByUniversityId;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.CreateUser;
using JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetWriterStatistics;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using JadaraITKnowledgeSystem.Domain.Quizzes;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entities;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Domain.Universities;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using JadaraITKnowledgeSystem.IntegrationTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace JadaraITKnowledgeSystem.IntegrationTests.Application;

[Collection(InfrastructureCollection.Name)]
public class HandlerTests(InfrastructureFixture database)
{
    [Fact]
    public async Task GetFacultiesByUniversityId_ReturnsOnlyThatUniversitysFaculties()
    {
        await using var context = database.CreateContext();
        var (university, faculty, _) = await context.SeedHierarchyAsync();
        var other = University.Create(TestData.Unique("Other University")).Value;
        context.Universities.Add(other);
        await context.SaveChangesAsync();
        context.Faculties.Add(Faculty.Create("Faculty of Law", other.Id).Value);
        await context.SaveChangesAsync();

        var result = await new GetFacultiesByUniversityIdQueryHandler(context, NullLogger<GetFacultiesByUniversityIdQueryHandler>.Instance)
            .Handle(new GetFacultiesByUniversityIdQuery(university.Id, 1, 10), default);

        Assert.Equal(faculty.Id, Assert.Single(result.Value.Items).Id);
    }

    [Fact]
    public async Task CreateCourse_WithoutCode_CreatesANewCourseInsteadOfReusingAnother()
    {
        await using var context = database.CreateContext();
        var (_, faculty, major) = await context.SeedHierarchyAsync();
        var otherMajor = Major.Create(TestData.Unique("Other Major"), faculty.Id).Value;
        context.Majors.Add(otherMajor);
        await context.SaveChangesAsync();
        var handler = new CreateCourseCommandHandler(context, NullLogger<CreateCourseCommandHandler>.Instance);

        var first = await handler.Handle(new CreateCourseCommand(major.Id, "Ethics", null, null, RequirementType.University, RequirementNature.Compulsory, 3), default);
        var second = await handler.Handle(new CreateCourseCommand(otherMajor.Id, "Statistics", null, null, RequirementType.Major, RequirementNature.Elective, 3), default);

        Assert.NotEqual(first.Value.Id, second.Value.Id);
        Assert.Equal("Statistics", second.Value.CourseName);
        Assert.Equal(otherMajor.Id, Assert.Single(second.Value.CourseRequirementMappings).MajorId);
    }

    [Fact]
    public async Task GetCourseById_IncludesTheMajorMappings()
    {
        await using var context = database.CreateContext();
        var (_, _, major) = await context.SeedHierarchyAsync();
        var created = await new CreateCourseCommandHandler(context, NullLogger<CreateCourseCommandHandler>.Instance)
            .Handle(new CreateCourseCommand(major.Id, "Networks", null, null, RequirementType.Major, RequirementNature.Compulsory, 3), default);

        await using var readContext = database.CreateContext();
        var result = await new GetCourseByIdQueryHandler(readContext, NullLogger<GetCourseByIdQueryHandler>.Instance)
            .Handle(new GetCourseByIdQuery(created.Value.Id), default);

        Assert.Single(result.Value.CourseRequirementMappings);
    }

    [Fact]
    public async Task CreateUser_WhenIdentityCreationFails_ReturnsTheErrorsAndRemovesTheDomainUser()
    {
        await using var context = database.CreateContext();
        var (_, _, major) = await context.SeedHierarchyAsync();
        var email = TestData.UniqueEmail();
        var identity = Substitute.For<IIdentityUserService>();
        identity.CreateAsync(default!, default!, default, default).ReturnsForAnyArgs(
            Task.FromResult<Result<int>>(new List<Error> { Error.Validation("DuplicateEmail", "Email is already taken.") }));

        var result = await new CreateUserCommandHandler(context, identity, NullLogger<CreateUserCommandHandler>.Instance)
            .Handle(new CreateUserCommand("New Student", email, major.Id, "Passw0rdX"), default);

        Assert.Equal("DuplicateEmail", result.TopError.Code);
        await identity.DidNotReceiveWithAnyArgs().AddToRoleAsync(default, default!);
        await using var verify = database.CreateContext();
        Assert.False(await verify.Users.AnyAsync(u => u.Email.Address == email.ToUpperInvariant()));
    }

    [Fact]
    public async Task WriterStatistics_CountsOnlyTheWritersOwnContent()
    {
        var writerEmail = TestData.UniqueEmail();
        await using var context = database.CreateContext(TestCurrentUser.InRole(Roles.Writer, email: writerEmail));
        var (_, _, major) = await context.SeedHierarchyAsync();
        var writer = await context.SeedUserAsync(major.Id, writerEmail, "Writer");
        var otherWriter = await context.SeedUserAsync(major.Id);

        var course = Course.Create(TestData.Unique("Databases"), 3).Value;
        context.Courses.Add(course);
        await context.SaveChangesAsync();
        // The audit interceptor stamps CreatedBy with the caller's email (as typed, lower-case here).
        context.CourseMaterials.Add(CourseMaterial.Create("Notes", "materials/1/a.pdf", "application/pdf", 10, course.Id).Value);
        context.Quizzes.Add(QuizWithOneQuestion(course.Id, writer.Id, choices: 3));
        context.Quizzes.Add(QuizWithOneQuestion(course.Id, otherWriter.Id, choices: 5));
        await context.SaveChangesAsync();

        var stats = (await new GetWriterStatisticsQueryHandler(context, TestCurrentUser.InRole(Roles.Writer, domainUserId: writer.Id))
            .Handle(new GetWriterStatisticsQuery(writer.Id), default)).Value;

        Assert.Equal(1, stats.TotalMaterials);
        Assert.Equal(1, stats.TotalQuizzes);
        Assert.Equal(1, stats.TotalQuizQuestions);
        Assert.Equal(3, stats.TotalQuizChoices);
    }

    [Fact]
    public async Task WriterStatistics_WriterCannotReadSomeoneElses()
    {
        await using var context = database.CreateContext();

        var result = await new GetWriterStatisticsQueryHandler(context, TestCurrentUser.InRole(Roles.Writer, domainUserId: 1))
            .Handle(new GetWriterStatisticsQuery(2), default);

        Assert.Equal(ErrorKind.Forbidden, result.TopError.Type);
    }

    private static Quiz QuizWithOneQuestion(int courseId, int writerId, int choices)
    {
        var quiz = Quiz.Create(courseId, writerId, $"Quiz by {writerId}").Value;
        var question = Question.Create(0, QuestionType.SingleChoice, "Question?").Value;
        for (var i = 0; i < choices; i++)
            question.AddChoice(Choice.Create(0, $"Choice {i}", i == 0).Value);
        quiz.AddQuestion(question);
        return quiz;
    }
}
