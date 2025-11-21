using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Interfaces
{
    public interface IFileManager
    {
        Task<string> UploadAsync(
            Stream fileStream,
            string extension,
            string folder,
            CancellationToken cancellationToken = default);

        Task<string> UpdateAsync(
            string? oldFileUrl,
            Stream fileStream,
            string extension,
            string folder,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            string? fileUrl,
            CancellationToken cancellationToken = default);

        Task<string> MoveFromTempToPermanentAsync(
            string tempFileUrl,
            string permanentFolder,
            CancellationToken cancellationToken = default);

        Task<int> DeleteOldTempFilesAsync(
        TimeSpan olderThan,
        CancellationToken cancellationToken = default);
    }
}
