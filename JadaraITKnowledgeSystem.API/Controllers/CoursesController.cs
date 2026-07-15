using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.AddCourseResource;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.AssignCourseToMajor;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CompleteCourse;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourse;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourseInfo;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourseMaterial;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateFolder;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.DeleteCourseResource;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.EnrollCourse;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.UpdateCourseInfo;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.UpdateCourseResource;
using JadaraITKnowledgeSystem.Application.Features.Courses.Models;
// TODO: Grade functionality is temporarily disabled.
// using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.AddGrade;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseByCode;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseById;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseContentByFolder;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseContentsByWriterId;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseInfoByCourseId;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseInfoByWriterId;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCoursesByMajorId;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetEnrolledCourses;
using JadaraITKnowledgeSystem.Domain.Courses.Enums; // added for enums
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/courses")]
[ApiController]
[Produces("application/json")]
public class CoursesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateCourseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: course => CreatedAtAction(nameof(GetById), new { id = course.Id }, course),
            onError: errors => BadRequest(new { errors })
        );
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetCourseByIdQuery(id),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: course => Ok(course),
            onError: errors => NotFound(new { errors })
        );
    }

    [HttpGet("by-code/{courseCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(
        string courseCode,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetCourseByCodeQuery(courseCode),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: course => Ok(course),
            onError: errors => NotFound(new { errors })
        );
    }

    [HttpGet("by-major/{majorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByMajorId(
        int majorId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] RequirementNature? requirementNature = null,
        [FromQuery] RequirementType? requirementType = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetCoursesByMajorIdQuery(majorId, pageNumber, pageSize, requirementNature, requirementType),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: summaries => Ok(summaries),
            onError: errors => BadRequest(new { errors })
        );
    }


    /// <summary>
    /// Assign an existing course to a major with requirement type and nature.
    /// </summary>
    [HttpPost("{courseId}/assign-to-major")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignCourseToMajor(
        int courseId,
        [FromBody] AssignCourseToMajorRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AssignCourseToMajorCommand(
            courseId,
            request.MajorId,
            request.RequirementType,
            request.RequirementNature);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: mapping => Created($"api/courses/{courseId}", mapping),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                return top.Code switch
                {
                    "Course.NotFound" or "Major.NotFound" => NotFound(new { errors }),
                    "Course.AlreadyAssigned" => Conflict(new { errors }),
                    _ => BadRequest(new { errors })
                };
            });
    }

    // ===== Course Info =====
    [HttpGet("{courseId}/info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseInfo(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetCourseInfoByCourseIdQuery(courseId),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: info => Ok(info),
            onError: errors => NotFound(new { errors })
        );
    }

    /// <summary>
    /// Get course info with resources created by the current writer.
    /// Only accessible to writers, admins, and superadmins.
    /// </summary>
    [HttpGet("{courseId}/my-info")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseInfoByWriter(
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetCourseInfoByWriterIdQuery(courseId),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: info => Ok(info),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                return top.Code switch
                {
                    "User.NotAuthenticated" => Unauthorized(new { errors }),
                    "CourseInfo.NotFound" => NotFound(new { errors }),
                    _ => BadRequest(new { errors })
                };
            });
    }

    public sealed record CreateCourseInfoRequest(
        DifficultyLevel DifficultyLevel,
        string? Description = null,
        string? DemonstrationVideoUrl = null,
        string? DemonstrationVideoTitle = null);

    [HttpPost("{courseId}/info")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCourseInfo(
        int courseId,
        [FromBody] CreateCourseInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCourseInfoCommand(
            courseId,
            request.DifficultyLevel,
            request.Description,
            request.DemonstrationVideoUrl,
            request.DemonstrationVideoTitle);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: info => CreatedAtAction(nameof(GetCourseInfo), new { courseId }, info),
            onError: errors => BadRequest(new { errors })
        );
    }

    public sealed record UpdateCourseInfoRequest(
        DifficultyLevel? DifficultyLevel = null,
        string? Description = null,
        string? DemonstrationVideoUrl = null,
        string? DemonstrationVideoTitle = null);

    [HttpPut("{courseId}/info")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCourseInfo(
        int courseId,
        [FromBody] UpdateCourseInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateCourseInfoCommand(
            courseId,
            request.DifficultyLevel,
            request.Description,
            request.DemonstrationVideoUrl,
            request.DemonstrationVideoTitle);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: info => Ok(info),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top.Code == "Course.NotFound" || top.Code == "CourseInfo.NotFound")
                    return NotFound(new { errors });
                return BadRequest(new { errors });
            });
    }

    // ===== Course Resources =====
    public sealed record AddCourseResourceRequest(
        string Title,
        ResourceType Type,
        string Url,
        string? Description = null,
        string? DemonstrationVideoUrl = null);

    [HttpPost("{courseId}/resources")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCourseResource(
        int courseId,
        [FromBody] AddCourseResourceRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddCourseResourceCommand(
            courseId,
            request.Title,
            request.Type,
            request.Url,
            request.Description,
            request.DemonstrationVideoUrl);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: resource => CreatedAtAction(nameof(GetCourseInfo), new { courseId }, resource),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top.Code == "Course.NotFound" || top.Code == "CourseInfo.NotFound")
                    return NotFound(new { errors });
                return BadRequest(new { errors });
            });
    }

    public sealed record UpdateCourseResourceRequest(
        string? Title = null,
        ResourceType? Type = null,
        string? Url = null,
        string? Description = null,
        string? DemonstrationVideoUrl = null);

    [HttpPut("{courseId}/resources/{resourceId}")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCourseResource(
        int courseId,
        int resourceId,
        [FromBody] UpdateCourseResourceRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateCourseResourceCommand(
            courseId,
            resourceId,
            request.Title,
            request.Type,
            request.Url,
            request.Description,
            request.DemonstrationVideoUrl);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: resource => Ok(resource),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top.Code == "Course.NotFound" || top.Code == "CourseInfo.NotFound" || top.Code == "Resource.NotFound")
                    return NotFound(new { errors });
                return BadRequest(new { errors });
            });
    }

    [HttpDelete("{courseId}/resources/{resourceId}")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourseResource(
        int courseId,
        int resourceId,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteCourseResourceCommand(courseId, resourceId);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: _ => NoContent(),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top.Code == "Course.NotFound" || top.Code == "CourseInfo.NotFound" || top.Code == "Resource.NotFound")
                    return NotFound(new { errors });
                return BadRequest(new { errors });
            });
    }

    // ===== Course Materials =====
    public sealed record CreateMaterialRequest(string Title, string ContemtUrl, int? FolderId, string? Description, List<string>? Tags);

    [HttpPost("{courseId}/materials")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMaterial(
        int courseId,
        [FromBody] CreateMaterialRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCourseMaterialCommand(
            request.Title,
            request.ContemtUrl,
            courseId,
            request.FolderId,
            request.Description,
            request.Tags);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: material => CreatedAtAction(nameof(GetById), new { id = material.CourseId }, material),
            onError: errors => BadRequest(new { errors })
        );
    }

    // ===== Folders =====
    public sealed record CreateFolderRequest(string Name, int? ParentFolderId, string? Description);

    [HttpPost("{courseId}/folders")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFolder(
        int courseId,
        [FromBody] CreateFolderRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateFolderCommand(
            request.Name,
            courseId,
            request.ParentFolderId,
            request.Description);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: folder => CreatedAtAction(nameof(GetById), new { id = folder.CourseId }, folder),
            onError: errors => BadRequest(new { errors })
        );
    }

    // ===== Course Contents (folders + materials at a level) =====
    [HttpGet("{courseId}/contents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContents(
        int courseId,
        [FromQuery] int? folderId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCourseContentsQuery(courseId, folderId);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match<IActionResult>(
            onValue: contents => Ok(contents),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top.Code == "Course.NotFound")
                    return NotFound(new { errors });
                return BadRequest(new { errors });
            });
    }

    /// <summary>
    /// Get course contents (folders and materials) created by the current writer.
    /// Only accessible to writers, admins, and superadmins.
    /// </summary>
    [HttpGet("{courseId}/my-contents")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContentsByWriter(
        int courseId,
        [FromQuery] int? folderId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCourseContentsByWriterIdQuery(courseId, folderId);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match<IActionResult>(
            onValue: contents => Ok(contents),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                return top.Code switch
                {
                    "User.NotAuthenticated" => Unauthorized(new { errors }),
                    "Course.NotFound" => NotFound(new { errors }),
                    _ => BadRequest(new { errors })
                };
            });
    }

    // ===== Enrollment Management =====

    public sealed record EnrollCourseRequest(string? Notes = null);

    /// <summary>
    /// Enroll the current user in a course.
    /// </summary>
    [HttpPost("{courseId}/enroll")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EnrollInCourse(
        int courseId,
        [FromBody] EnrollCourseRequest? request,
        CancellationToken cancellationToken = default)
    {
        var command = new EnrollCourseCommand(courseId, request?.Notes);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: enrollment => CreatedAtAction(nameof(GetEnrolledCourses), null, enrollment),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                return top.Code switch
                {
                    "User.NotAuthenticated" => Unauthorized(new { errors }),
                    "Course.NotFound" or "User.NotFound" => NotFound(new { errors }),
                    "Enrollment.AlreadyExists" => Conflict(new { errors }),
                    _ => BadRequest(new { errors })
                };
            });
    }

    /// <summary>
    /// Get courses the current user is enrolled in.
    /// </summary>
    [HttpGet("my-enrollments")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEnrolledCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isFinished = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetEnrolledCoursesQuery(pageNumber, pageSize, isFinished);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match<IActionResult>(
            onValue: enrollments => Ok(enrollments),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top.Code == "User.NotAuthenticated")
                    return Unauthorized(new { errors });
                return BadRequest(new { errors });
            });
    }

    // TODO: Grade functionality is temporarly disabled.
    // Universities may have different grading systems (A, A+, B, etc.)
    // This needs to be redesigned to support flexible grading systems.
    // public sealed record AddGradeRequest(decimal Grade);
    // 
    // /// <summary>
    // /// Add or update a grade for a finished course enrollment.
    // /// </summary>
    // [HttpPut("{courseId}/grade")]
    // [Authorize]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // public async Task<IActionResult> AddGrade(
    //     int courseId,
    //     [FromBody] AddGradeRequest request,
    //     CancellationToken cancellationToken = default)
    // {
    //     var command = new AddGradeCommand(courseId, request.Grade);
    //     var result = await _mediator.Send(command, cancellationToken);
    // 
    //     return result.Match<IActionResult>(
    //         onValue: enrollment => Ok(enrollment),
    //         onError: errors =>
    //         {
    //             var top = errors.FirstOrDefault();
    //             return top.Code switch
    //             {
    //                 "User.NotAuthenticated" => Unauthorized(new { errors }),
    //                 "Enrollment.NotFound" => NotFound(new { errors }),
    //                 "Enrollment.NotFinished" => BadRequest(new { errors }),
    //                 _ => BadRequest(new { errors })
    //             };
    //         });
    // }
}
