using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Storage
{
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
    }

}
