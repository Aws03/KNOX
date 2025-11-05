using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services
{
    public interface IStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string? folder = null);
        Task<bool> DeleteAsync(string fileName, string? folder = null);
        string GetFileUrl(string fileName, string? folder = null);
    }
}
