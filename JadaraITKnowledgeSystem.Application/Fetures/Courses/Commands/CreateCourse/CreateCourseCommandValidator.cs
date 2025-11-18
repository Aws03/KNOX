using FluentValidation;
using JadaraITKnowledgeSystem.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourse
{
    public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateCourseCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.CourseName)
                .NotEmpty().WithMessage("Course name is required.")
                .MaximumLength(150);

            RuleFor(x => x.MajorId).GreaterThan(0);

            RuleFor(x => x.Credits)
                .InclusiveBetween(0, 30)
                .When(x => x.Credits.HasValue);

            RuleFor(x => x.CourseName)
                .MustAsync(BeUniqueWithinUniversity)
                .WithMessage("A course with the same name or code already exists in this university.");
        }

        private async Task<bool> BeUniqueWithinUniversity(CreateCourseCommand cmd, string courseName, CancellationToken ct)
        {

            var universityId = await _context.Majors
                .Where(m => m.Id == cmd.MajorId)
                .Select(m => m.Faculty.University.Id)
                .FirstOrDefaultAsync(ct);

            return !await _context.MajorCourses
                .AnyAsync(m =>
                    (m.Course.CourseName == cmd.CourseName ||
                     (!string.IsNullOrWhiteSpace(cmd.CourseCode) && m.Course.CourseCode == cmd.CourseCode))
                    && m.Major.Faculty.University.Id == universityId,
                    ct);

        }
    }
}
