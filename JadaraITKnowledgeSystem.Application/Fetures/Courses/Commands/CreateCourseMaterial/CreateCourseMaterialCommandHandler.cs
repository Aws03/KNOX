using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseMaterial
{
    public sealed class CreateCourseMaterialHandler(
    IApplicationDbContext context,
    IStorageService storage,
    ILogger<CreateCourseMaterialHandler> logger)
    : IRequestHandler<CreateCourseMaterialCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            CreateCourseMaterialCommand request,
            CancellationToken cancellationToken)
        {
            // 1️⃣ Create path
            string path = $"course-materials/{request.CourseId}";

            // 2️⃣ Upload file
            string uniqueName = $"{Guid.NewGuid()}-{request.File.FileName}";
            string url;

            using (var stream = request.File.OpenReadStream())
            {
                url = await storage.UploadAsync(stream, uniqueName, path, cancellationToken);
            }

            // 3️⃣ Create entity
            var materialResult = CourseMaterial.Create(
                request.Title,
                url,
                request.CourseId,
                request.Description);

            if (materialResult.IsFailure)
                return Result.Failure<int>(materialResult.Error);

            var entity = materialResult.Value;

            // 4️⃣ Save to DB
            context.CourseMaterials.Add(entity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Course material {Id} created for course {CourseId}.", entity.Id, request.CourseId);

            return Result.Success(entity.Id);
        }
    }

}
