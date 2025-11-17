using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.FileMangment
{
    public class FileManager : IFileManager
    {
        private readonly IStorageService _storage;

        private static readonly HashSet<string> _allowedExtensions =
        [
            ".jpg", ".jpeg", ".png", ".webp", ".pdf", ".mp4" , ".docx" , ".pptx" , ".pptm"
        ];

        public FileManager(IStorageService storage)
        {
            _storage = storage;
        }

        public async Task<string> UploadAsync(
            Stream fileStream,
            string extension,
            string folder,
            CancellationToken cancellationToken = default)
        {
            ValidateExtension(extension);

            string fileName = GenerateFileName(extension);

            string fileUrl = await _storage.UploadAsync(
                fileStream,
                fileName,
                folder,
                cancellationToken);

            return fileUrl;
        }

        public async Task<string> UpdateAsync(
            string? oldFileUrl,
            Stream newFileStream,
            string extension,
            string folder,
            CancellationToken cancellationToken = default)
        {
            ValidateExtension(extension);

            // Upload new file
            string newUrl = await UploadAsync(
                newFileStream,
                extension,
                folder,
                cancellationToken);

            // Delete old file if exists
            if (!string.IsNullOrWhiteSpace(oldFileUrl))
            {
                string? oldFileName = ExtractFileName(oldFileUrl);
                if (oldFileName != null)
                {
                    await _storage.DeleteAsync(oldFileName, folder, cancellationToken);
                }
            }

            return newUrl;
        }

        public async Task<bool> DeleteAsync(
            string? fileUrl,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return false;

            string? fileName = ExtractFileName(fileUrl);
            if (fileName == null)
                return false;

            // fileUrl usually contains folder, e.g. .../courses/12/materials/abc.png
            string? folder = ExtractFolder(fileUrl);

            return await _storage.DeleteAsync(fileName, folder, cancellationToken);
        }


        // =============================
        // Helpers
        // =============================

        private string GenerateFileName(string extension)
            => $"{Guid.NewGuid():N}{extension}";

        private void ValidateExtension(string extension)
        {
            if (!_allowedExtensions.Contains(extension.ToLower()))
                throw new InvalidOperationException($"Extension '{extension}' is not allowed.");
        }

        private string? ExtractFileName(string fileUrl)
        {
            try
            {
                return fileUrl.Split('/').LastOrDefault();
            }
            catch
            {
                return null;
            }
        }

        private string? ExtractFolder(string fileUrl)
        {
            try
            {
                var parts = fileUrl.Split('/').Reverse().Skip(1).Reverse();
                return string.Join("/", parts.Skip(parts.Count() - 2));
            }
            catch
            {
                return null;
            }
        }
    }
}
