using FluentValidation;
using JadaraITKnowledgeSystem.Application.Common.Behaviours;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.CreateQuiz;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Universities.Queries.GetUniversities;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.ChangePassword;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.CreateUser;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.ResetPassword;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.UnitTests.Application;

public class ValidationTests
{
    public sealed record SampleCommand(string Name) : IRequest<Result<int>>;

    private sealed class SampleCommandValidator : AbstractValidator<SampleCommand>
    {
        public SampleCommandValidator() => RuleFor(x => x.Name).NotEmpty();
    }

    [Fact]
    public async Task ValidationBehavior_InvalidRequest_ShortCircuitsWithValidationErrors()
    {
        var behavior = new ValidationBehavior<SampleCommand, Result<int>>([new SampleCommandValidator()]);
        var handlerCalled = false;

        var result = await behavior.Handle(new SampleCommand(""), _ => { handlerCalled = true; return Task.FromResult<Result<int>>(1); }, default);

        Assert.False(handlerCalled);
        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
        Assert.Equal("Name", result.TopError.Code);
    }

    [Fact]
    public async Task ValidationBehavior_ValidRequest_CallsTheHandler()
    {
        var behavior = new ValidationBehavior<SampleCommand, Result<int>>([new SampleCommandValidator()]);

        var result = await behavior.Handle(new SampleCommand("ok"), _ => Task.FromResult<Result<int>>(7), default);

        Assert.Equal(7, result.Value);
    }

    [Theory]
    [InlineData(0, 10, false)]
    [InlineData(1, 0, false)]
    [InlineData(1, 51, false)]
    [InlineData(1, 50, true)]
    public void PaginatedQueries_EnforcePageBounds(int pageNumber, int pageSize, bool valid)
    {
        Assert.Equal(valid, new GetUniversitiesQueryValidator().Validate(new GetUniversitiesQuery(pageNumber, pageSize)).IsValid);
    }

    [Fact]
    public void CreateQuizValidator_RejectsQuestionsWithoutTextOrWithAnInvalidType()
    {
        var command = new CreateQuizCommand(
            "Quiz", WriterId: 1, CourseId: 1, Description: null,
            Questions:
            [
                new CreateQuestionDto { Text = "", Type = QuestionType.SingleChoice },
                new CreateQuestionDto { Text = "What?", Type = (QuestionType)0 }
            ],
            Tags: null);

        var result = new CreateQuizCommandValidator().Validate(command);

        Assert.Contains(result.Errors, e => e.PropertyName == "Questions[0].Text");
        Assert.Contains(result.Errors, e => e.PropertyName == "Questions[1].Type");
    }

    [Theory]
    [InlineData("short1A", false)]
    [InlineData("alllowercase1", false)]
    [InlineData("NoDigitsHere", false)]
    [InlineData("Passw0rdX", true)]
    [InlineData("Passw0rd!", true)]
    public void PasswordPolicy_IsTheSameForRegistrationResetAndChange(string password, bool valid)
    {
        Assert.Equal(valid, new CreateUserCommandValidator().Validate(new CreateUserCommand("Name", "a@b.co", 1, password)).IsValid);
        Assert.Equal(valid, new ResetPasswordCommandValidator().Validate(new ResetPasswordCommand("a@b.co", "123456", password)).IsValid);
        Assert.Equal(valid, new ChangePasswordCommandValidator().Validate(new ChangePasswordCommand("Old0Password", password)).IsValid);
    }
}
