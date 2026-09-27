using System.ComponentModel.DataAnnotations;

namespace JadaraITKnowledgeSystem.Infrastructure.Options;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>Public origin the browser uses to fetch uploads, e.g. https://knox.example.com.</summary>
    [Required, Url]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Directory holding uploaded files; relative paths resolve against the content root.</summary>
    public string RootPath { get; set; } = "wwwroot/uploads";

    /// <summary>URL path the uploads are served under (and that file URLs are built with).</summary>
    public const string RequestPath = "/uploads";

    public string ResolveRootPath(string contentRootPath) =>
        Path.GetFullPath(Path.IsPathRooted(RootPath) ? RootPath : Path.Combine(contentRootPath, RootPath));
}
