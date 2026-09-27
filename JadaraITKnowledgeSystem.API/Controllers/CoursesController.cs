using JadaraITKnowledgeSystem.API.Contracts;
using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Security;
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
using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseByCode;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseById;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseContents;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseContentsByWriterId;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseInfoByCourseId;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseInfoByWriterId;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCoursesByMajorId;
using JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetEnrolledCourses;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/courses")]
public sealed class CoursesController(ISender sender) : ApiControllerBase(sender)
{
    /// <summary>Creates a course and assigns it to a major with its requirement type and nature.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<CourseDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateCourseCommand command, CancellationToken cancellationToken) =>
        ResultOrProblem(await Sender.Send(command, cancellationToken),
            course => CreatedAtAction(nameof(GetById), new { id = course.Id }, course));

    /// <summary>Gets a course by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<CourseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetCourseByIdQuery(id), cancellationToken));

    /// <summary>Gets a course by its code (e.g. CS101); 404 when no course has it.</summary>
    [HttpGet("by-code/{courseCode}")]
    [ProducesResponseType<CourseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string courseCode, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetCourseByCodeQuery(courseCode), cancellationToken));

    /// <summary>Pages through a major's courses, optionally filtered by requirement type and nature.</summary>
    [HttpGet("by-major/{majorId:int}")]
    [ProducesResponseType<PaginatedList<CourseSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMajorId(
        int majorId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] RequirementNature? requirementNature = null,
        [FromQuery] RequirementType? requirementType = null,
        CancellationToken cancellationToken = default) =>
        OkOrProblem(await Sender.Send(
            new GetCoursesByMajorIdQuery(majorId, pageNumber, pageSize, requirementNature, requirementType), cancellationToken));

    /// <summary>Assigns an existing course to a (further) major.</summary>
    [HttpPost("{courseId:int}/assign-to-major")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<CourseRequirementMappingDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> AssignCourseToMajor(
        int courseId, [FromBody] AssignCourseToMajorRequest request, CancellationToken cancellationToken) =>
        ResultOrProblem(
            await Sender.Send(new AssignCourseToMajorCommand(courseId, request.MajorId, request.RequirementType, request.RequirementNature), cancellationToken),
            mapping => CreatedAtAction(nameof(GetById), new { id = courseId }, mapping));

    // ===== Course info =====

    /// <summary>Gets the course's description and learning resources.</summary>
    [HttpGet("{courseId:int}/info")]
    [ProducesResponseType<CourseInfoDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseInfo(int courseId, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetCourseInfoByCourseIdQuery(courseId), cancellationToken));

    /// <summary>Course info with only the resources the calling writer created.</summary>
    [HttpGet("{courseId:int}/my-info")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<CourseInfoDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseInfoByWriter(int courseId, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetCourseInfoByWriterIdQuery(courseId), cancellationToken));

    /// <summary>Creates the course description.</summary>
    [HttpPost("{courseId:int}/info")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<CourseInfoDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCourseInfo(int courseId, [FromBody] CreateCourseInfoRequest request, CancellationToken cancellationToken) =>
        ResultOrProblem(
            await Sender.Send(new CreateCourseInfoCommand(courseId, request.DifficultyLevel, request.Description,
                request.DemonstrationVideoUrl, request.DemonstrationVideoTitle), cancellationToken),
            info => CreatedAtAction(nameof(GetCourseInfo), new { courseId }, info));

    /// <summary>Updates the course description.</summary>
    [HttpPut("{courseId:int}/info")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<CourseInfoDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCourseInfo(int courseId, [FromBody] UpdateCourseInfoRequest request, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new UpdateCourseInfoCommand(courseId, request.DifficultyLevel, request.Description,
            request.DemonstrationVideoUrl, request.DemonstrationVideoTitle), cancellationToken));

    // ===== Course resources =====

    /// <summary>Adds a learning resource (link) to the course.</summary>
    [HttpPost("{courseId:int}/resources")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<CourseResourceDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddCourseResource(int courseId, [FromBody] AddCourseResourceRequest request, CancellationToken cancellationToken) =>
        ResultOrProblem(
            await Sender.Send(new AddCourseResourceCommand(courseId, request.Title, request.Type, request.Url,
                request.Description, request.DemonstrationVideoUrl), cancellationToken),
            resource => CreatedAtAction(nameof(GetCourseInfo), new { courseId }, resource));

    /// <summary>Updates a learning resource.</summary>
    [HttpPut("{courseId:int}/resources/{resourceId:int}")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<CourseResourceDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCourseResource(
        int courseId, int resourceId, [FromBody] UpdateCourseResourceRequest request, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new UpdateCourseResourceCommand(courseId, resourceId, request.Title, request.Type,
            request.Url, request.Description, request.DemonstrationVideoUrl), cancellationToken));

    /// <summary>Deletes a learning resource.</summary>
    [HttpDelete("{courseId:int}/resources/{resourceId:int}")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCourseResource(int courseId, int resourceId, CancellationToken cancellationToken) =>
        NoContentOrProblem(await Sender.Send(new DeleteCourseResourceCommand(courseId, resourceId), cancellationToken));

    // ===== Materials and folders =====

    /// <summary>Creates a course material from a finished direct upload (see POST api/files/material-uploads). Returns the material with a signed contentUrl.</summary>
    [HttpPost("{courseId:int}/materials")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<CourseMaterialDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateMaterial(int courseId, [FromBody] CreateMaterialRequest request, CancellationToken cancellationToken) =>
        ResultOrProblem(
            await Sender.Send(new CreateCourseMaterialCommand(request.Title, request.UploadKey, courseId, request.FolderId,
                request.Description, request.Tags), cancellationToken),
            material => CreatedAtAction(nameof(GetContents), new { courseId, folderId = material.FolderId }, material));

    /// <summary>Creates a folder, optionally inside another folder of the same course.</summary>
    [HttpPost("{courseId:int}/folders")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<FolderDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateFolder(int courseId, [FromBody] CreateFolderRequest request, CancellationToken cancellationToken) =>
        ResultOrProblem(
            await Sender.Send(new CreateFolderCommand(request.Name, courseId, request.ParentFolderId, request.Description), cancellationToken),
            folder => CreatedAtAction(nameof(GetContents), new { courseId, folderId = folder.Id }, folder));

    /// <summary>Folders and materials at one level of the course tree (root when folderId is omitted).</summary>
    [HttpGet("{courseId:int}/contents")]
    [Authorize] // hands out signed URLs for private course files
    [ProducesResponseType<CourseContentsDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContents(int courseId, [FromQuery] int? folderId, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetCourseContentsQuery(courseId, folderId), cancellationToken));

    /// <summary>Like <c>contents</c>, limited to what the calling writer created.</summary>
    [HttpGet("{courseId:int}/my-contents")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<CourseContentsDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContentsByWriter(int courseId, [FromQuery] int? folderId, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetCourseContentsByWriterIdQuery(courseId, folderId), cancellationToken));

    // ===== Enrollment =====

    /// <summary>Enrolls the caller in the course.</summary>
    [HttpPost("{courseId:int}/enroll")]
    [Authorize]
    [ProducesResponseType<EnrollmentDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Enroll(int courseId, [FromBody] EnrollCourseRequest? request, CancellationToken cancellationToken) =>
        ResultOrProblem(
            await Sender.Send(new EnrollCourseCommand(courseId, request?.Notes), cancellationToken),
            enrollment => CreatedAtAction(nameof(GetEnrolledCourses), null, enrollment));

    /// <summary>Marks the caller's enrollment in the course as finished.</summary>
    [HttpPost("{courseId:int}/complete")]
    [Authorize]
    [ProducesResponseType<EnrollmentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(int courseId, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new CompleteCourseCommand(courseId), cancellationToken));

    /// <summary>Pages through the caller's enrollments, optionally only finished or unfinished ones.</summary>
    [HttpGet("my-enrollments")]
    [Authorize]
    [ProducesResponseType<PaginatedList<EnrolledCourseSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrolledCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isFinished = null,
        CancellationToken cancellationToken = default) =>
        OkOrProblem(await Sender.Send(new GetEnrolledCoursesQuery(pageNumber, pageSize, isFinished), cancellationToken));
}
