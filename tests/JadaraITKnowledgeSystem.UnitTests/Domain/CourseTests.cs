using System.Reflection;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;

namespace JadaraITKnowledgeSystem.UnitTests.Domain;

public class CourseTests
{
    // Folder/requirement rules depend on ids EF would assign; set them the way EF does.
    private static T WithId<T>(T entity, int id) where T : BaseEntity
    {
        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity, id);
        return entity;
    }

    private static Course PersistedCourse() => WithId(Course.Create("Data Structures", 3, courseCode: "CS201").Value, 10);

    [Fact]
    public void AddFolder_DuplicateNameAtSameLevel_IsAConflict()
    {
        var course = PersistedCourse();
        course.AddFolder("Lectures");

        var result = course.AddFolder("lectures");

        Assert.True(result.IsError);
        Assert.Equal("Folder.DuplicateName", result.TopError.Code);
    }

    [Fact]
    public void AddFolder_UnknownParent_IsNotFound()
    {
        var course = PersistedCourse();

        var result = course.AddFolder("Week 1", parentFolderId: 99);

        Assert.True(result.IsError);
        Assert.Equal("Folder.ParentNotFound", result.TopError.Code);
    }

    [Fact]
    public void RemoveFolder_WithSubFolders_RequiresDeleteContents()
    {
        var course = PersistedCourse();
        var parent = WithId(course.AddFolder("Lectures").Value, 1);
        WithId(course.AddFolder("Week 1", parentFolderId: parent.Id).Value, 2);

        var refused = course.RemoveFolder(parent.Id);
        var removed = course.RemoveFolder(parent.Id, deleteContents: true);

        Assert.Equal("Folder.HasSubFolders", refused.TopError.Code);
        Assert.True(removed.IsSuccess);
        Assert.Empty(course.Folders);
    }

    [Fact]
    public void ValidateFolderHierarchy_MovingFolderUnderItsDescendant_IsRejected()
    {
        var course = PersistedCourse();
        var root = WithId(course.AddFolder("Root").Value, 1);
        var child = WithId(course.AddFolder("Child", root.Id).Value, 2);

        var result = course.ValidateFolderHierarchy(root.Id, child.Id);

        Assert.True(result.IsError);
        Assert.Equal("Folder.CircularReference", result.TopError.Code);
    }

    [Fact]
    public void AssignToMajor_SameMajorTwice_IsAConflict()
    {
        var course = PersistedCourse();
        course.AssignToMajor(5, RequirementType.Major, RequirementNature.Compulsory);

        var result = course.AssignToMajor(5, RequirementType.Major, RequirementNature.Elective);

        Assert.True(result.IsError);
        Assert.Equal("Course.AlreadyAssigned", result.TopError.Code);
        Assert.Single(course.Requirements);
    }

    [Fact]
    public void SetCourseInfo_BeforeCourseIsPersisted_IsRejected()
    {
        var course = Course.Create("Algorithms", 3).Value;

        var result = course.SetCourseInfo(DifficultyLevel.Easy);

        Assert.True(result.IsError);
        Assert.Equal("Course.NotPersisted", result.TopError.Code);
    }

    [Theory]
    [InlineData("https://host/uploads/permanent/material/notes.pdf", true)]
    [InlineData("https://host/uploads/permanent/material/slides.PPTX", true)]
    [InlineData("https://host/uploads/permanent/material/intro.docx", true)]
    [InlineData("https://host/uploads/permanent/material/video.mp4", false)]
    public void CourseMaterial_SupportsTextExtraction_OnlyForDocumentFormats(string url, bool expected)
    {
        var material = CourseMaterial.Create("Notes", url, courseId: 1).Value;

        Assert.Equal(expected, material.SupportsTextExtraction());
    }

    [Fact]
    public void Enrollment_CompletingTwice_IsAConflict()
    {
        var enrollment = Enrollment.Create(userId: 1, courseId: 1).Value;
        enrollment.Complete();

        var result = enrollment.Complete();

        Assert.True(enrollment.IsFinished);
        Assert.NotNull(enrollment.FinishedAt);
        Assert.Equal("Enrollment.AlreadyFinished", result.TopError.Code);
    }
}
