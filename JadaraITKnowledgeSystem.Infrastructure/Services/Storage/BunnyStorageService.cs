using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Storage
{
    public class BunnyStorageService : IStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _storageZoneName;
        private readonly string _apiKey;
        private readonly string _cdnBaseUrl;

        public BunnyStorageService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            var section = configuration.GetSection("Storage");
            _storageZoneName = section["StorageZoneName"] ?? throw new ArgumentNullException("Storage:StorageZoneName");
            _apiKey = section["ApiKey"] ?? throw new ArgumentNullException("Storage:ApiKey");
            _cdnBaseUrl = section["StorageZoneBaseUrl"] ?? throw new ArgumentNullException("Storage:StorageZoneBaseUrl");

            _httpClient.BaseAddress = new Uri("https://storage.bunnycdn.com/");
            _httpClient.DefaultRequestHeaders.Remove("AccessKey");
            _httpClient.DefaultRequestHeaders.Add("AccessKey", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string? folder = null)
        {
            try
            {
                var path = string.IsNullOrEmpty(folder)
                ? $"{_storageZoneName}/{fileName}"
                : $"{_storageZoneName}/{folder}/{fileName}";

                using var content = new StreamContent(fileStream);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var response = await _httpClient.PutAsync(path, content);

                response.EnsureSuccessStatusCode();

                return GetFileUrl(fileName, folder);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                throw new Exception("Error uploading file to BunnyCDN", ex);
            }

        }

        public async Task<bool> DeleteAsync(string fileName, string? folder = null)
        {
            var path = string.IsNullOrEmpty(folder)
                ? $"{_storageZoneName}/{fileName}"
                : $"{_storageZoneName}/{folder}/{fileName}";

            var response = await _httpClient.DeleteAsync(path);

            return response.IsSuccessStatusCode;
        }

        public string GetFileUrl(string fileName, string? folder = null)
        {
            return string.IsNullOrEmpty(folder)
                ? $"{_cdnBaseUrl}/{fileName}"
                : $"{_cdnBaseUrl}/{folder}/{fileName}";
        }
    }
}



