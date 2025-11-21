using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Storage;

public class BunnyStorageService : IStorageService
{
    private readonly HttpClient _httpClient;
    private readonly string _zoneName;
    private readonly string _apiKey;
    private readonly string _cdnBaseUrl;

    public BunnyStorageService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;

        var section = config.GetSection("Storage");

        _zoneName = section["StorageZoneName"]
            ?? throw new ArgumentNullException("Storage:StorageZoneName");

        _apiKey = section["ApiKey"]
            ?? throw new ArgumentNullException("Storage:ApiKey");

        _cdnBaseUrl = section["StorageZoneBaseUrl"]
            ?? throw new ArgumentNullException("Storage:StorageZoneBaseUrl");

        _httpClient.BaseAddress = new Uri("https://storage.bunnycdn.com/");
        _httpClient.DefaultRequestHeaders.Add("AccessKey", _apiKey);
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        var fullPath = BuildPath(fileName, path);

        using var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var response = await _httpClient.PutAsync(fullPath, content, cancellationToken);

        response.EnsureSuccessStatusCode();

        return GetFileUrl(fileName, path);
    }

    public async Task<bool> DeleteAsync(
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        var fullPath = BuildPath(fileName, path);

        var response = await _httpClient.DeleteAsync(fullPath, cancellationToken);

        return response.IsSuccessStatusCode;
    }

    public string GetFileUrl(string fileName, string? path = null)
    {
        return string.IsNullOrEmpty(path)
            ? $"{_cdnBaseUrl}/{fileName}"
            : $"{_cdnBaseUrl}/{path}/{fileName}";
    }

    private string BuildPath(string fileName, string? folder)
        => string.IsNullOrEmpty(folder)
            ? $"{_zoneName}/{fileName}"
            : $"{_zoneName}/{folder}/{fileName}";

    public async Task<List<StorageFileInfo>> ListFilesAsync(
    string? path,
    CancellationToken cancellationToken = default)
    {
        var allFiles = new List<StorageFileInfo>();

        await ListFilesRecursiveAsync(path, allFiles, cancellationToken);

        return allFiles;
    }

    private async Task ListFilesRecursiveAsync(
        string? path,
        List<StorageFileInfo> accumulator,
        CancellationToken cancellationToken)
    {
        // Bunny requires trailing slash for directories
        var fullPath = string.IsNullOrEmpty(path)
            ? $"{_zoneName}/"
            : $"{_zoneName}/{path.TrimEnd('/')}/";

        var request = new HttpRequestMessage(HttpMethod.Get, fullPath);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Path doesn't exist, return empty
                return;
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception(
                $"Failed to list files. Status: {response.StatusCode}. Body: {error}"
            );
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var files = System.Text.Json.JsonSerializer.Deserialize<List<StorageFileInfo>>(
            json,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<StorageFileInfo>();

        foreach (var file in files)
        {
            if (file.IsDirectory)
            {
                // Recursively get files from subdirectory
                var subPath = string.IsNullOrEmpty(path)
                    ? file.ObjectName
                    : $"{path}/{file.ObjectName}";

                await ListFilesRecursiveAsync(subPath, accumulator, cancellationToken);
            }
            else
            {
                // Add file to results
                accumulator.Add(file);
            }
        }
    }

    public async Task<Stream?> DownloadAsync(
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        // Build full path: /{zone}/{path}/{fileName}
        var fullPath = BuildPath(fileName, path);

        // Prepare request (AccessKey already added in HttpClient constructor)
        var request = new HttpRequestMessage(HttpMethod.Get, fullPath);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead, // stream-safe
            cancellationToken
        );

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null; // file does not exist

        response.EnsureSuccessStatusCode();

        // Return the file stream
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
