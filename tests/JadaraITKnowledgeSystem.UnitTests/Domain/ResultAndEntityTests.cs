using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entities;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Domain.Universities;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;

namespace JadaraITKnowledgeSystem.UnitTests.Domain;

public class ResultAndEntityTests
{
    [Fact]
    public void Result_FromValue_IsSuccess()
    {
        Result<int> result = 42;

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Result_FromError_ExposesTheErrorAndDefaultValue()
    {
        Result<string> result = Error.NotFound("Thing.NotFound", "Missing");

        Assert.True(result.IsError);
        Assert.Null(result.Value);
        Assert.Equal("Thing.NotFound", result.TopError.Code);
        Assert.Equal(ErrorKind.NotFound, result.TopError.Type);
    }

    [Fact]
    public void Result_Failure_WithNoErrors_Throws()
    {
        Assert.Throws<ArgumentException>(() => Result<int>.Failure([]));
    }

    [Fact]
    public void Result_Match_RoutesToTheRightBranch()
    {
        Result<int> ok = 1;
        Result<int> failed = Error.Validation("X", "bad");

        Assert.Equal("value:1", ok.Match(v => $"value:{v}", _ => "error"));
        Assert.Equal("error:X", failed.Match(_ => "value", e => $"error:{e[0].Code}"));
    }

    [Fact]
    public void University_Create_WithBlankName_UsesAProperErrorCode()
    {
        var result = University.Create(" ");

        Assert.Equal("University.Name.Required", result.TopError.Code);
    }

    [Fact]
    public void Major_UpdateName_RejectsWhitespaceAndOverlongNames()
    {
        var major = Major.Create("Computer Science", 1).Value;

        var blank = Assert.Throws<ArgumentException>(() => major.UpdateName("   "));
        var tooLong = Assert.Throws<ArgumentException>(() => major.UpdateName(new string('x', 121)));

        Assert.Equal("name", blank.ParamName);
        Assert.Equal("name", tooLong.ParamName);
    }

    [Fact]
    public void QuizGenerationJob_TracksGeneratedQuizIdsAndTruncatesLongErrors()
    {
        var job = QuizGenerationJob.Create(1, 1, 1, new QuizGenerationOptions { MaxQuizzes = 2 }).Value;

        job.AddGeneratedQuizId(3);
        job.AddGeneratedQuizId(4);
        job.MarkFailed(new string('e', 2_000));

        Assert.Equal(new[] { 3, 4 }, job.GetGeneratedQuizIds());
        Assert.Equal(QuizGenerationStatus.Failed, job.Status);
        Assert.Equal(1_000, job.ErrorMessage!.Length);
        Assert.Equal(2, job.GetOptions().MaxQuizzes);
    }
}
