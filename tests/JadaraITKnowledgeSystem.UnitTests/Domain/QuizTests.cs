using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;

namespace JadaraITKnowledgeSystem.UnitTests.Domain;

public class QuizTests
{
    private static Quiz NewQuiz(IEnumerable<string>? tags = null) =>
        Quiz.Create(courseId: 1, writerId: 1, title: "Intro quiz", tags: tags).Value;

    private static UserReaction Reaction(int userId, ReactionType type) =>
        UserReaction.Create(userId, quizId: 1, type).Value;

    [Fact]
    public void Create_WithBlankTitle_ReturnsValidationError()
    {
        var result = Quiz.Create(1, 1, "  ");

        Assert.True(result.IsError);
        Assert.Equal("Quiz_Title_Required", result.TopError.Code);
    }

    [Fact]
    public void Create_DefaultsToManualSource()
    {
        var quiz = NewQuiz();

        Assert.Equal(QuizSource.Manual, quiz.Source);
        Assert.Null(quiz.SourceMaterialId);
    }

    [Fact]
    public void AddReaction_CountsLikesAndDislikesPerUser()
    {
        var quiz = NewQuiz();

        quiz.AddReaction(Reaction(1, ReactionType.Like));
        quiz.AddReaction(Reaction(2, ReactionType.Dislike));

        Assert.Equal(1, quiz.Likes);
        Assert.Equal(1, quiz.Dislikes);
    }

    [Fact]
    public void AddReaction_SwitchingReaction_MovesTheCount()
    {
        var quiz = NewQuiz();
        quiz.AddReaction(Reaction(1, ReactionType.Like));

        var result = quiz.AddReaction(Reaction(1, ReactionType.Dislike));

        Assert.True(result.IsSuccess);
        Assert.Equal(0, quiz.Likes);
        Assert.Equal(1, quiz.Dislikes);
        Assert.Single(quiz.Reactions);
    }

    [Fact]
    public void AddReaction_SameReactionTwice_IsAConflict()
    {
        var quiz = NewQuiz();
        quiz.AddReaction(Reaction(1, ReactionType.Like));

        var result = quiz.AddReaction(Reaction(1, ReactionType.Like));

        Assert.True(result.IsError);
        Assert.Equal(ErrorKind.Conflict, result.TopError.Type);
        Assert.Equal(1, quiz.Likes);
    }

    [Fact]
    public void UpdateTags_TrimsAndDeduplicatesCaseInsensitively()
    {
        var quiz = NewQuiz([" csharp ", "CSharp", "linq"]);

        Assert.Equal(new[] { "csharp", "linq" }, quiz.Tags);
    }

    [Fact]
    public void UpdateTags_MoreThanTenDistinctTags_IsRejected()
    {
        var result = Quiz.Create(1, 1, "quiz", tags: Enumerable.Range(1, 11).Select(i => $"tag{i}"));

        Assert.True(result.IsError);
        Assert.Equal("Quiz.Tag.Limit", result.TopError.Code);
    }

    [Fact]
    public void AddOrUpdateAttempt_KeepsOneAttemptPerUser()
    {
        var quiz = NewQuiz();

        quiz.AddOrUpdateAttempt(QuizAttempt.Create(1, userId: 7, score: 40).Value);
        quiz.AddOrUpdateAttempt(QuizAttempt.Create(1, userId: 7, score: 90).Value);

        var attempt = Assert.Single(quiz.Attempts);
        Assert.Equal(90, attempt.Score);
    }

    [Fact]
    public void MarkAsAiGenerated_RecordsSourceAndPart()
    {
        var quiz = NewQuiz();

        var result = quiz.MarkAsAiGenerated(sourceMaterialId: 12, partNumber: 2, totalParts: 3);

        Assert.True(result.IsSuccess);
        Assert.Equal(QuizSource.AIGenerated, quiz.Source);
        Assert.Equal(12, quiz.SourceMaterialId);
        Assert.Equal(2, quiz.PartNumber);
        Assert.Equal(3, quiz.TotalParts);
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(5, 0, 1)]
    [InlineData(5, 3, 2)]
    public void MarkAsAiGenerated_WithInvalidArguments_LeavesQuizManual(int materialId, int part, int total)
    {
        var quiz = NewQuiz();

        var result = quiz.MarkAsAiGenerated(materialId, part, total);

        Assert.True(result.IsError);
        Assert.Equal(QuizSource.Manual, quiz.Source);
    }

    [Fact]
    public void Question_Create_WithBlankText_IsRejected()
    {
        var result = Question.Create(quizId: 0, QuestionType.SingleChoice, " ");

        Assert.True(result.IsError);
        Assert.Equal("Question_Text_Required", result.TopError.Code);
    }

    [Fact]
    public void QuizAttempt_ScoreOutsideZeroToHundred_Throws()
    {
        Assert.Throws<ArgumentException>(() => QuizAttempt.Create(1, 1, 101));
    }
}
